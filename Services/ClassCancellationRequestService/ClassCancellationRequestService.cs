using System.Net;
using griffined_api.Dtos.ClassCancellationRequestDto;
using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Extensions.DateTimeExtensions;
using griffined_api.Dtos.ScheduleDtos;

namespace griffined_api.Services.ClassCancellationRequestService
{
    public class ClassCancellationRequestService : IClassCancellationRequestService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;
        private readonly ITeacherService _teacherService;

        public ClassCancellationRequestService(DataContext context, IFirebaseService firebaseService, ITeacherService teacherService)
        {
            _context = context;
            _firebaseService = firebaseService;
            _teacherService = teacherService;
        }

        public async Task<ServiceResponse<string>> AddClassCancellationRequest(int studyClassId)
        {
            var dbStudyClass = await _context.StudyClasses
                                             .Include(x => x.Schedule)
                                             .Include(c => c.StudyCourse)
                                             .Include(c => c.StudySubject)
                                             .Include(c => c.Teacher)
                                             .FirstOrDefaultAsync(c => c.Id == studyClassId && c.Status == ClassStatus.NONE)
                                             ?? throw new NotFoundException($"StudyClass that can cancel with ID {studyClassId} is not found.");

            if (dbStudyClass.StudyCourse.StudyCourseType == StudyCourseType.Group)
                throw new BadRequestException($"Group Class cannot cancel");

            // GET THE CURRENT DATE.
            DateTime currentTime = DateTime.Now;

            // COMBINE THE CURRENT DATE WITH CLASS START TIME TO GET DATETIME OBJECT.
            DateTime classStartTime = currentTime.Date + dbStudyClass.Schedule.FromTime;

            // CALCULATE THE TIME DIFFERENCE BETWEEN THE CLASS START TIME AND THE CURRENT TIME.
            TimeSpan timeDifference = classStartTime - currentTime;

            // CHECK FOR 17 HOURS LIMIT.
            if (Math.Abs(timeDifference.TotalHours) > 17)
            {
                throw new ConflictException("Class cancellation can only be requested outside ±17 hours period of the class start time.");
            }

            var classCancellationRequest = new ClassCancellationRequest
            {
                RequestedDate = DateTime.Now,
                StudyClass = dbStudyClass,
                StudyCourse = dbStudyClass.StudyCourse,
                StudySubject = dbStudyClass.StudySubject,
            };

            var role = _firebaseService.GetRoleWithToken();
            var userId = _firebaseService.GetAzureIdWithToken();

            if (role == "student")
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == userId)
                                    ?? throw new NotFoundException($"Student with ID {userId} is not found.");

                classCancellationRequest.Student = dbStudent;
                classCancellationRequest.RequestedRole = CancellationRole.Student;
            }
            else
            {
                var dbTeacher = await _context.Teachers.Include(x => x.Mandays)
                                                        .ThenInclude(x => x.WorkTimes)
                                                       .FirstOrDefaultAsync(s => s.Id == userId)
                                                       ?? throw new NotFoundException($"Teacher with ID {userId} is not found.");

                classCancellationRequest.Teacher = dbTeacher;
                classCancellationRequest.RequestedRole = CancellationRole.Teacher;
            }

            dbStudyClass.Status = ClassStatus.PENDING_CANCELLATION;

            _context.ClassCancellationRequests.Add(classCancellationRequest);

            var message = $"Your class on {dbStudyClass.Schedule.Date.ToDateString()} has been cancelled.";

            var teacherNotification = new TeacherNotification
            {
                Teacher = dbStudyClass.Teacher,
                StudyCourse = dbStudyClass.StudyCourse,
                Title = "Your Class Has Been Cancelled.",
                Message = message,
                DateCreated = DateTime.Now,
                Type = TeacherNotificationType.ClassCancellation,
                HasRead = false
            };

            _context.TeacherNotifications.Add(teacherNotification);

            var dbEAs = await _context.Staff
                        .Where(s => s.Role == "ea")
                        .ToListAsync();

            foreach (var ea in dbEAs)
            {
                var eaNotification = new StaffNotification
                {
                    Staff = ea,
                    CancellationRequest = classCancellationRequest,
                    Title = "New Class Cancellation Request",
                    Message = "A new class cancellation request has been requested. Click here for more details.",
                    DateCreated = DateTime.Now,
                    Type = StaffNotificationType.ClassCancellationRequest,
                    HasRead = false
                };

                _context.StaffNotifications.Add(eaNotification);
            }

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<List<ClassCancellationRequestResponseDto>>> ListAllClassCancellationRequest()
        {
            var dbRequests = await _context.ClassCancellationRequests
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.Subject)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudySubjectMember)
                            .Include(r => r.StudyClass)
                                .ThenInclude(c => c.Schedule)
                            .Include(r => r.Student)
                            .Include(r => r.Teacher)
                            .ToListAsync();


            var data = new List<ClassCancellationRequestResponseDto>();
            foreach (var dbRequest in dbRequests)
            {
                var cancellationRequestDto = new ClassCancellationRequestResponseDto
                {
                    RequestId = dbRequest.Id,
                    StudyCourseId = dbRequest.StudyCourse.Id,
                    Section = dbRequest.StudyCourse.Section,
                    RequestedRole = dbRequest.RequestedRole,
                    Course = dbRequest.StudyCourse.Course.course,
                    Level = dbRequest.StudyCourse.Level?.level,
                    StudyCourseType = dbRequest.StudyCourse.StudyCourseType,
                    RequestedDate = dbRequest.RequestedDate.ToDateTimeString(),
                    CancelledDate = dbRequest.StudyClass.Schedule.Date.ToDateString(),
                    CancelledFromTime = dbRequest.StudyClass.Schedule.FromTime.ToTimeSpanString(),
                    CancelledToTime = dbRequest.StudyClass.Schedule.ToTime.ToTimeSpanString(),
                    Status = dbRequest.Status,
                };

                if (dbRequest.RequestedRole == CancellationRole.Student)
                {
                    if (dbRequest.Student == null)
                        throw new NotFoundException("Student is not found.");

                    foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                    {
                        if (dbStudySubject.StudySubjectMember.Any(m => m.StudentId == dbRequest.StudentId))
                        {
                            cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                            {
                                StudySubjectId = dbStudySubject.Id,
                                SubjectId = dbStudySubject.Subject.Id,
                                Subject = dbStudySubject.Subject.subject,
                            });
                        }
                    }

                    cancellationRequestDto.RequestedBy = new RequestedByResponseDto
                    {
                        UserId = dbRequest.Student.Id,
                        FirstName = dbRequest.Student.FirstName,
                        LastName = dbRequest.Student.LastName,
                        Nickname = dbRequest.Student.Nickname,
                    };
                }
                else
                {
                    if (dbRequest.Teacher == null)
                        throw new NotFoundException("Teacher is not found.");

                    foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                    {
                        cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                        });
                    }

                    cancellationRequestDto.RequestedBy = new RequestedByResponseDto
                    {
                        UserId = dbRequest.Teacher.Id,
                        FirstName = dbRequest.Teacher.FirstName,
                        LastName = dbRequest.Teacher.LastName,
                        Nickname = dbRequest.Teacher.Nickname,
                    };
                }

                if (dbRequest.TakenByEAId != null)
                {
                    var takenByEA = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.TakenByEAId)
                                    ?? throw new InternalServerException("Something went wrong with Taken By Staff");

                    cancellationRequestDto.TakenByEA = new StaffNameOnlyResponseDto
                    {
                        StaffId = takenByEA.Id,
                        FullName = takenByEA.FirstName + " " + takenByEA.LastName,
                        Nickname = takenByEA.Nickname,
                    };
                }

                data.Add(cancellationRequestDto);
            }

            var response = new ServiceResponse<List<ClassCancellationRequestResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
            return response;
        }

        public async Task<ServiceResponse<ClassCancellationRequestDetailResponseDto>> GetClassCancellationRequestDetailByRequestId(int requestId)
        {
            var dbRequest = await _context.ClassCancellationRequests
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.Subject)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudyClasses)
                                        .ThenInclude(c => c.Schedule)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudyClasses)
                                        .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudyClasses)
                                        .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudySubjectMember)
                            .Include(r => r.StudyClass)
                                .ThenInclude(c => c.Schedule)
                            .Include(r => r.StudySubject)
                                .ThenInclude(s => s.Subject)
                            .Include(r => r.Student)
                            .Include(r => r.Teacher)
                            .Where(x => x.StudyCourse.StudySubjects.Any(y => y.StudyClasses.Any(z => z.Status != ClassStatus.DELETED)))
                            .FirstOrDefaultAsync(r => r.Id == requestId)
                            ?? throw new NotFoundException($"Class Cancellation Request with ID {requestId} is not found.");

            var cancellationRequestDto = new ClassCancellationRequestDetailResponseDto
            {
                RequestId = dbRequest.Id,
                StudyCourseId = dbRequest.StudyCourse.Id,
                Section = dbRequest.StudyCourse.Section,
                RequestedRole = dbRequest.RequestedRole,
                CourseId = dbRequest.StudyCourse.Course.Id,
                Course = dbRequest.StudyCourse.Course.course,
                Level = dbRequest.StudyCourse.Level?.level,
                StudyCourseType = dbRequest.StudyCourse.StudyCourseType,
                StartDate = dbRequest.StudyCourse.StartDate.ToDateString(),
                EndDate = dbRequest.StudyCourse.EndDate.ToDateString(),
                Method = dbRequest.StudyCourse.Method,
                TotalHour = dbRequest.StudyCourse.TotalHour,
                RequestedDate = dbRequest.RequestedDate.ToDateTimeString(),
                Status = dbRequest.Status,
                RejectedReason = dbRequest.RejectedReason,
                RequestedClass = new CancellationInfoResponseDto
                {
                    ClassId = dbRequest.StudyClass.Id,
                    ClassNo = dbRequest.StudyClass.ClassNumber,
                    StudyCourseId = dbRequest.StudyCourse.Id,
                    CourseId = dbRequest.StudyCourse.Course.Id,
                    Course = dbRequest.StudyCourse.Course.course,
                    StudySubjectId = dbRequest.StudySubject.Id,
                    SubjectId = dbRequest.StudySubject.Subject.Id,
                    Subject = dbRequest.StudySubject.Subject.subject,
                    Date = dbRequest.StudyClass.Schedule.Date.ToDateString(),
                    FromTime = dbRequest.StudyClass.Schedule.FromTime.ToTimeSpanString(),
                    ToTime = dbRequest.StudyClass.Schedule.ToTime.ToTimeSpanString(),
                    TeacherFirstName = dbRequest.StudyClass.Teacher.FirstName,
                    TeacherLastName = dbRequest.StudyClass.Teacher.LastName,
                    TeacherNickname = dbRequest.StudyClass.Teacher.Nickname,
                },
            };

            if (dbRequest.RequestedRole == CancellationRole.Student)
            {
                if (dbRequest.Student == null)
                    throw new NotFoundException("Student is not found.");

                foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                {
                    if (dbStudySubject.StudySubjectMember.Any(m => m.StudentId == dbRequest.StudentId))
                    {
                        cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        });
                    }
                }

                cancellationRequestDto.RequestedBy = new RequestedByWithContactResponseDto
                {
                    UserId = dbRequest.Student.Id,
                    FirstName = dbRequest.Student.FirstName,
                    LastName = dbRequest.Student.LastName,
                    Nickname = dbRequest.Student.Nickname,
                    Phone = dbRequest.Student.Phone,
                    Email = dbRequest.Student.Email,
                    Line = dbRequest.Student.Line,
                };
            }
            else
            {
                if (dbRequest.Teacher == null)
                    throw new NotFoundException("Teacher is not found.");

                foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                {
                    cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                    {
                        StudySubjectId = dbStudySubject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject,
                    });
                }

                cancellationRequestDto.RequestedBy = new RequestedByWithContactResponseDto
                {
                    UserId = dbRequest.Teacher.Id,
                    FirstName = dbRequest.Teacher.FirstName,
                    LastName = dbRequest.Teacher.LastName,
                    Nickname = dbRequest.Teacher.Nickname,
                    Phone = dbRequest.Teacher.Phone,
                    Email = dbRequest.Teacher.Email,
                    Line = dbRequest.Teacher.Line,
                };
            }

            if (dbRequest.TakenByEAId != null)
            {
                var takenByEA = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.TakenByEAId)
                                ?? throw new InternalServerException("Something went wrong with Taken By Staff");

                cancellationRequestDto.TakenByEA = new StaffNameOnlyResponseDto
                {
                    StaffId = takenByEA.Id,
                    FullName = takenByEA.FirstName + " " + takenByEA.LastName,
                    Nickname = takenByEA.Nickname,
                };
            }

            var rawSchedules = new List<ScheduleResponseDto>();

            foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
            {
                if (dbRequest.RequestedRole == CancellationRole.Student
                && !dbStudySubject.StudySubjectMember.Any(s => s.StudentId == dbRequest.StudentId))
                {
                    continue;
                }
                foreach (var dbStudyClass in dbStudySubject.StudyClasses.Where(sc => sc.Status != ClassStatus.DELETED))
                {
                    var schedule = new ScheduleResponseDto
                    {
                        StudyCourseId = dbStudySubject.StudyCourse.Id,
                        Day = dbStudyClass.Schedule.Date.DayOfWeek.ToString().ToUpper(),
                        StudyClassId = dbStudyClass.Id,
                        ClassNo = dbStudyClass.ClassNumber,
                        Room = dbStudyClass.Schedule.Room,
                        Date = dbStudyClass.Schedule.Date.ToDateString(),
                        FromTime = dbStudyClass.Schedule.FromTime,
                        ToTime = dbStudyClass.Schedule.ToTime,
                        CourseSubject = dbRequest.StudyCourse.Course.course + " "
                                            + dbStudySubject.Subject.subject
                                            + " " + (dbRequest.StudyCourse.Level?.level ?? ""),
                        CourseId = dbRequest.StudyCourse.Course.Id,
                        CourseName = dbRequest.StudyCourse.Course.course,
                        StudySubjectId = dbStudySubject.Subject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        SubjectName = dbStudySubject.Subject.subject,
                        Teacher = new TeacherNameResponseDto
                        {
                            TeacherId = dbStudyClass.Teacher.Id,
                            FirstName = dbStudyClass.Teacher.FirstName,
                            LastName = dbStudyClass.Teacher.LastName,
                            Nickname = dbStudyClass.Teacher.Nickname,
                        },
                        ClassStatus = dbStudyClass.Status,
                    };

                    foreach (var dbTeacherShift in dbStudyClass.TeacherShifts)
                    {
                        if (dbTeacherShift.TeacherWorkType != TeacherWorkType.NORMAL)
                            schedule.AdditionalHours = new AdditionalHours
                            {
                                Hours = dbTeacherShift.Hours,
                                TeacherWorkType = dbTeacherShift.TeacherWorkType,
                            };
                    }

                    rawSchedules.Add(schedule);
                }
            }

            cancellationRequestDto.Schedules = rawSchedules.OrderBy(s => (s.Date + " " + s.FromTime).ToDateTime()).ToList();

            var response = new ServiceResponse<ClassCancellationRequestDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = cancellationRequestDto,
            };
            return response;
        }

        public async Task<ServiceResponse<string>> EaTakeRequest(int requestId)
        {
            var dbRequest = await _context.ClassCancellationRequests.FirstOrDefaultAsync(c => c.Id == requestId)
                            ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            if (dbRequest.TakenByEAId != null)
                throw new ConflictException($"Request Already Taken By Another EA");

            var eaId = _firebaseService.GetAzureIdWithToken();

            dbRequest.TakenByEAId = eaId;

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<string>> EaReleaseRequest(int requestId)
        {
            var dbRequest = await _context.ClassCancellationRequests.FirstOrDefaultAsync(c => c.Id == requestId)
                            ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            if (dbRequest.TakenByEAId == null)
                throw new ConflictException("EA haven't take this request yet.");

            dbRequest.TakenByEAId = null;
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateScheduleWithClassCancellationRequest(int requestId, UpdateStudyCourseRequestDto updateRequest)
        {
            var staffId = _firebaseService.GetAzureIdWithToken();
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == staffId) ?? throw new NotFoundException("No Staff Found");

            var dbRemoveStudyClasses = await _context.StudyClasses
                                        .Include(c => c.StudyCourse)
                                            .ThenInclude(sc => sc.Course)
                                        .Include(c => c.StudySubject)
                                            .ThenInclude(ss => ss.Subject)
                                        .Include(c => c.Schedule)
                                        .Include(c => c.Teacher)
                                        .Include(c => c.Attendances)
                                            .ThenInclude(a => a.Student)
                                        .Where(c => updateRequest.RemoveStudyClassId.Contains(c.Id)).ToListAsync();


            var dbTeachers = await _context.Teachers
                            .Include(t => t.Mandays)
                                .ThenInclude(x => x.WorkTimes)
                            .ToListAsync();

            foreach (var dbRemoveStudyClass in dbRemoveStudyClasses)
            {
                // FIND IF IT IS SUBSTITUTE CLASS OR NOT
                var duplicateClassTime = updateRequest.NewSchedule.FirstOrDefault(x =>
                                                                    x.StudySubjectId == dbRemoveStudyClass.StudySubjectId &&
                                                                    x.Date.ToDateTime() == dbRemoveStudyClass.Schedule.Date &&
                                                                    x.FromTime.ToTimeSpan() == dbRemoveStudyClass.Schedule.FromTime
                                                                    && x.ToTime.ToTimeSpan() == dbRemoveStudyClass.Schedule.ToTime
                                                                );

                var removeHistory = new StudyCourseHistory
                {
                    StudyCourse = dbRemoveStudyClass.StudyCourse,
                    Staff = staff,
                    UpdatedDate = DateTime.Now,
                    Type = StudyCourseHistoryType.Schedule,
                    StudyClass = dbRemoveStudyClass,
                };

                // IF IS SUBSTITUTE CLASS
                if (duplicateClassTime != null)
                {
                    dbRemoveStudyClass.Schedule.CalendarType = DailyCalendarType.SUBSTITUTE;
                    dbRemoveStudyClass.IsSubstitute = true;

                    var newTeacher = dbTeachers.FirstOrDefault(x => x.Id == duplicateClassTime.TeacherId)
                                                        ?? throw new NotFoundException("Teacher not found");

                    var description = $"Update teacher {dbRemoveStudyClass.StudyCourse.Course.course} {dbRemoveStudyClass.StudySubject.Subject.subject} on {dbRemoveStudyClass.Schedule.Date.ToDateWithDayString()} ({dbRemoveStudyClass.Schedule.FromTime.ToTimeSpanString()} - {dbRemoveStudyClass.Schedule.ToTime.ToTimeSpanString()}) from Teacher {dbRemoveStudyClass.Teacher.Nickname} to {newTeacher.Nickname}.";

                    // UPDATE SUBSTITUTED TEACHER IN HISTORY DESCRIPTION
                    removeHistory.Description = description;

                    // REMOVE PREVIOUS TEACHER SHIFT
                    foreach (var shift in dbRemoveStudyClass.TeacherShifts)
                    {
                        dbRemoveStudyClass.TeacherShifts.Remove(shift);
                    }

                    // ADD NEW TEACHER SHIFT
                    var worktypes = _teacherService.GetTeacherWorkTypesWithHours(newTeacher, dbRemoveStudyClass.Schedule.Date, dbRemoveStudyClass.Schedule.FromTime, dbRemoveStudyClass.Schedule.ToTime);
                    foreach (var worktype in worktypes)
                    {
                        dbRemoveStudyClass.TeacherShifts ??= new List<TeacherShift>();
                        dbRemoveStudyClass.TeacherShifts.Add(new TeacherShift
                        {
                            Teacher = newTeacher,
                            TeacherWorkType = worktype.TeacherWorkType,
                            Hours = worktype.Hours,
                        });
                    }

                    // REMOVE SUBSTITUTE CLASS FROM NEW SCHEDULE
                    updateRequest.NewSchedule.Remove(duplicateClassTime);

                    // NOTIFY SUBSTITUTE TEACHER
                    var notification = new TeacherNotification
                    {
                        Teacher = newTeacher,
                        StudyCourse = dbRemoveStudyClass.StudyCourse,
                        Title = "You have been assigned to a new class as substitute teacher.",
                        Message = $"You have been assigned to a new class on {dbRemoveStudyClass.StudyCourse.Course.course} {dbRemoveStudyClass.StudySubject.Subject.subject} on {dbRemoveStudyClass.Schedule.Date.ToDateWithDayString()} ({dbRemoveStudyClass.Schedule.FromTime.ToTimeSpanString()} - {dbRemoveStudyClass.Schedule.ToTime.ToTimeSpanString()}).",
                        DateCreated = DateTime.Now,
                        Type = TeacherNotificationType.MakeupClass,
                        HasRead = false
                    };
                    _context.TeacherNotifications.Add(notification);

                    // NOTIFY EVERY STUDENT IN THAT CLASS
                    foreach (var attendance in dbRemoveStudyClass.Attendances)
                    {
                        if (attendance.Student != null)
                        {
                            var studentNotification = new StudentNotification
                            {
                                Student = attendance.Student!,
                                StudyCourse = dbRemoveStudyClass.StudyCourse,
                                Title = "Your Teacher Has Been Updated.",
                                Message = $"Your class on {dbRemoveStudyClass.Schedule.Date.ToDateString()} has been substituted by {newTeacher.Nickname}.",
                                DateCreated = DateTime.Now,
                                Type = StudentNotificationType.ClassCancellation,
                                HasRead = false
                            };

                            _context.StudentNotifications.Add(studentNotification);
                        }
                    }

                }
                // IF IT IS NOT SUBSTITUTE CLASS
                else
                {
                    dbRemoveStudyClass.Schedule.CalendarType = DailyCalendarType.CANCELLED_CLASS;
                    dbRemoveStudyClass.Status = ClassStatus.CANCELLED;

                    var description = $"Cancelled {dbRemoveStudyClass.StudyCourse.Course.course} {dbRemoveStudyClass.StudySubject.Subject.subject} on {dbRemoveStudyClass.Schedule.Date.ToDateWithDayString()} ({dbRemoveStudyClass.Schedule.FromTime.ToTimeSpanString()} - {dbRemoveStudyClass.Schedule.ToTime.ToTimeSpanString()}) taught by Teacher {dbRemoveStudyClass.Teacher.Nickname}.";

                    // UPDATE CANCELLED CLASS HISTORY
                    removeHistory.Description = description;

                    // NOTIFY EVERY STUDENT IN THAT CLASS
                    foreach (var attendance in dbRemoveStudyClass.Attendances)
                    {
                        if (attendance.Student != null)
                        {
                            var studentNotification = new StudentNotification
                            {
                                Student = attendance.Student!,
                                StudyCourse = dbRemoveStudyClass.StudyCourse,
                                Title = "Your Class Has Been Cancelled.",
                                Message = $"Your class on {dbRemoveStudyClass.Schedule.Date.ToDateString()} has been cancelled.",
                                DateCreated = DateTime.Now,
                                Type = StudentNotificationType.ClassCancellation,
                                HasRead = false
                            };

                            _context.StudentNotifications.Add(studentNotification);
                        }
                    }
                }

                // INSERT REMOVE HISTORY TO DB
                dbRemoveStudyClass.StudyCourse.StudyCourseHistories ??= new List<StudyCourseHistory>();
                dbRemoveStudyClass.StudyCourse.StudyCourseHistories.Add(removeHistory);

                // NOTIFY OLD TEACHER
                var teacherNotification = new TeacherNotification
                {
                    Teacher = dbRemoveStudyClass.Teacher,
                    StudyCourse = dbRemoveStudyClass.StudyCourse,
                    Title = "Your Class Has Been Cancelled.",
                    Message = $"Your class on {dbRemoveStudyClass.Schedule.Date.ToDateString()} has been cancelled.",
                    DateCreated = DateTime.Now,
                    Type = TeacherNotificationType.ClassCancellation,
                    HasRead = false
                };

                _context.TeacherNotifications.Add(teacherNotification);

            }

            var dbStudySubjects = await _context.StudySubjects
                            .Include(s => s.StudyCourse)
                                .ThenInclude(sc => sc.Course)
                            .Include(s => s.Subject)
                            .Include(s => s.StudyClasses)
                                .ThenInclude(s => s.Teacher)
                            .Include(s => s.StudyCourse)
                            .Include(s => s.StudySubjectMember)
                                .ThenInclude(s => s.Student)
                            .Where(s => updateRequest.StudySubjectIds.Contains(s.Id))
                            .ToListAsync();

            foreach (var dbStudySubject in dbStudySubjects)
            {
                int studyClassCount = updateRequest.NewSchedule.Where(x => x.StudySubjectId == dbStudySubject.Id).Count();

                foreach (var newSchedule in updateRequest.NewSchedule.Where(s => s.StudySubjectId == dbStudySubject.Id))
                {
                    var classCount = dbStudySubject.StudyClasses.Count;
                    var dbTeacher = dbTeachers.FirstOrDefault(t => t.Id == newSchedule.TeacherId)
                                ?? throw new NotFoundException($"Teacher ID {newSchedule.TeacherId} is not found.");
                    var studyClass = new StudyClass
                    {
                        //TODO Class Count
                        Teacher = dbTeacher,
                        StudyCourse = dbStudySubject.StudyCourse,
                        IsMakeup = true,
                        Schedule = new Schedule
                        {
                            Date = newSchedule.Date.ToDateTime(),
                            FromTime = newSchedule.FromTime.ToTimeSpan(),
                            ToTime = newSchedule.ToTime.ToTimeSpan(),
                            Type = ScheduleType.Class,
                            CalendarType = DailyCalendarType.MAKEUP_CLASS,
                        },
                    };

                    var worktypes = _teacherService.GetTeacherWorkTypesWithHours(dbTeacher, newSchedule.Date.ToDateTime(), newSchedule.FromTime.ToTimeSpan(), newSchedule.ToTime.ToTimeSpan());
                    foreach (var worktype in worktypes)
                    {
                        studyClass.TeacherShifts ??= new List<TeacherShift>();
                        studyClass.TeacherShifts.Add(new TeacherShift
                        {
                            Teacher = dbTeacher,
                            TeacherWorkType = worktype.TeacherWorkType,
                            Hours = worktype.Hours,
                        });
                    }

                    foreach (var dbMember in dbStudySubject.StudySubjectMember)
                    {
                        studyClass.Attendances ??= new List<StudentAttendance>();
                        studyClass.Attendances.Add(new StudentAttendance
                        {
                            Attendance = Attendance.None,
                            Student = dbMember.Student,
                        });

                        var message = $"You have been assigned to a make up class on {dbStudySubject.StudyCourse.Course.course} {dbStudySubject.Subject.subject} on {newSchedule.Date.ToDateTime().ToDateWithDayString()} ({newSchedule.FromTime.ToTimeSpan().ToTimeSpanString()} - {newSchedule.ToTime.ToTimeSpan().ToTimeSpanString()}).";

                        var makeupClassStudentNotification = new StudentNotification
                        {
                            Student = dbMember.Student,
                            StudyCourse = dbStudySubject.StudyCourse,
                            Title = "You have been assigned to a make up class",
                            Message = message,
                            DateCreated = DateTime.Now,
                            Type = StudentNotificationType.MakeupClass,
                            HasRead = false
                        };

                        _context.StudentNotifications.Add(makeupClassStudentNotification);

                        var allStudyClasses = await _context.StudyClasses
                            .Include(sc => sc.Schedule)
                            .Where(sc => sc.Attendances.Any(a => a.StudentId == dbMember.Student.Id))
                            .ToListAsync();

                        var lastClassEndDate = allStudyClasses.Max(sc => sc.Schedule.Date);
                        var expiryDate = lastClassEndDate.AddDays(14);

                        dbMember.Student.ExpiryDate = expiryDate;
                    }
                    dbStudySubject.StudyClasses ??= new List<StudyClass>();
                    dbStudySubject.StudyClasses.Add(studyClass);

                    var addHistory = new StudyCourseHistory
                    {
                        StudyCourse = dbStudySubject.StudyCourse,
                        Staff = staff,
                        UpdatedDate = DateTime.Now,
                        Type = StudyCourseHistoryType.Schedule,
                        StudyClass = studyClass,
                    };

                    string addedStudyClassHistoryDescription = $"Added Makeup Class {dbStudySubject.StudyCourse.Course.course} {dbStudySubject.Subject.subject} on {newSchedule.Date.ToDateTime().ToDateWithDayString()} ({newSchedule.FromTime.ToTimeSpan().ToTimeSpanString()} - {newSchedule.ToTime.ToTimeSpan().ToTimeSpanString()}) taught by Teacher {dbTeachers.FirstOrDefault(t => t.Id == newSchedule.TeacherId)!.Nickname}.";

                    addHistory.Description = addedStudyClassHistoryDescription;
                    studyClass.StudyCourse.StudyCourseHistories ??= new List<StudyCourseHistory>();
                    studyClass.StudyCourse.StudyCourseHistories.Add(addHistory);
                }
            }

            var dbRequest = await _context.ClassCancellationRequests
                            .Include(r => r.Student)
                            .Include(r => r.Teacher)
                            .Include(r => r.StudyCourse)
                            .Include(r => r.StudyClass)
                                .ThenInclude(sc => sc.Schedule)
                            .FirstOrDefaultAsync(r => r.Id == requestId) ?? throw new NotFoundException("Request is not found.");
            dbRequest.Status = ClassCancellationRequestStatus.Completed;

            await _context.SaveChangesAsync();

            foreach (var studySubject in dbStudySubjects)
            {
                int c = 0;

                var studyClasses = studySubject.StudyClasses
                                               .OrderBy(sc => sc.Schedule.Date)
                                               .ThenBy(sc => sc.Schedule.FromTime)
                                               .Where(x => x.Status != ClassStatus.DELETED && x.Status != ClassStatus.CANCELLED)
                                               .ToList();

                var studyClassCount = studyClasses.Count;

                foreach (var studyClass in studyClasses)
                {
                    c++;
                    studyClass.IsFiftyPercent = false;
                    studyClass.IsHundredPercent = false;
                    if (c == studyClassCount / 2)
                    {
                        studyClass.IsFiftyPercent = true;
                        studyClass.IsHundredPercent = false;
                    }

                    if (c == studyClassCount)
                    {
                        studyClass.IsFiftyPercent = false;
                        studyClass.IsHundredPercent = true;
                    }
                }

                for (int i = 0; i < studyClasses.Count; i++)
                {
                    studyClasses[i].ClassNumber = i + 1;
                }

                foreach (var studyClass in studySubject.StudyClasses.Where(x => x.Status == ClassStatus.DELETED || x.Status == ClassStatus.CANCELLED))
                {
                    studyClass.IsFiftyPercent = false;
                    studyClass.IsHundredPercent = false;
                }
            }

            await _context.SaveChangesAsync();



            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        public async Task<ServiceResponse<string>> RejectRequest(int requestId, string rejectedReason)
        {
            var dbRequest = await _context.ClassCancellationRequests
                            .Include(r => r.Student)
                            .Include(r => r.Teacher)
                            .Include(r => r.StudyCourse)
                            .FirstOrDefaultAsync(r => r.Id == requestId
                            && r.Status == ClassCancellationRequestStatus.None)
                            ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            if (dbRequest.TakenByEAId == null)
                throw new ConflictException("EA hasn't taken this request yet.");

            if (dbRequest.TakenByEAId != _firebaseService.GetAzureIdWithToken())
                throw new ConflictException("You don't have permission to reject this request.");

            dbRequest.RejectedReason = rejectedReason;
            dbRequest.Status = ClassCancellationRequestStatus.Rejected;

            if (dbRequest.RequestedRole == CancellationRole.Student)
            {
                var classCancellationStudentNotification = new StudentNotification
                {
                    Student = dbRequest.Student!,
                    StudyCourse = dbRequest.StudyCourse,
                    Title = "Cancellation Request Rejected",
                    Message = "Your class cancellation request has been rejected.",
                    DateCreated = DateTime.Now,
                    Type = StudentNotificationType.ClassCancellation,
                    HasRead = false
                };

                _context.StudentNotifications.Add(classCancellationStudentNotification);
            }
            else
            {
                var classCancellationTeacherNotification = new TeacherNotification
                {
                    Teacher = dbRequest.Teacher!,
                    StudyCourse = dbRequest.StudyCourse,
                    Title = "Cancellation Request Rejected",
                    Message = "Your class cancellation request has been rejected.",
                    DateCreated = DateTime.Now,
                    Type = TeacherNotificationType.ClassCancellation,
                    HasRead = false
                };

                _context.TeacherNotifications.Add(classCancellationTeacherNotification);
            }

            var studyClass = await _context.StudyClasses.FirstOrDefaultAsync(x => x.Id == dbRequest.StudyClassId) ?? throw new NotFoundException("Study class not found.");

            studyClass.Status = ClassStatus.NONE;

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }
    }
}