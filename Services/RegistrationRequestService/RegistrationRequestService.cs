using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.RegistrationRequestDto;
using griffined_api.Dtos.ScheduleDtos;
using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Extensions.DateTimeExtensions;
using System.Net;

namespace griffined_api.Services.RegistrationRequestService
{
    public class RegistrationRequestService : IRegistrationRequestService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;
        private readonly ITeacherService _teacherService;

        public RegistrationRequestService(DataContext context, IFirebaseService firebaseService, ITeacherService teacherService)
        {
            _context = context;
            _firebaseService = firebaseService;
            _teacherService = teacherService;
        }

        public async Task<ServiceResponse<String>> AddNewRequestedCourses(NewCoursesRequestDto newRequestedCourses)
        {
            var response = new ServiceResponse<String>();
            var request = new RegistrationRequest();

            if (newRequestedCourses.MemberIds == null || newRequestedCourses.MemberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }
            foreach (var memberId in newRequestedCourses.MemberIds)
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == memberId) ?? throw new NotFoundException($"Student with ID {memberId} not found.");

                if (dbStudent.Status == StudentStatus.Inactive)
                {
                    dbStudent.Status = StudentStatus.OnProcess;
                }

                var member = new RegistrationRequestMember
                {
                    Student = dbStudent
                };

                request.RegistrationRequestMembers ??= new List<RegistrationRequestMember>();
                request.RegistrationRequestMembers.Add(member);
            }

            if (newRequestedCourses.Type == StudyCourseType.Private && newRequestedCourses.MemberIds.Count == 1)
            {
                var student = request.RegistrationRequestMembers.ElementAt(0).Student;
                request.Section = student.Nickname + "/" + student.FirstName;
            }
            else if (newRequestedCourses.Section != null && newRequestedCourses.Section != "" && newRequestedCourses.Type != StudyCourseType.Private)
                request.Section = newRequestedCourses.Section;
            else
                throw new BadRequestException("Missing Section Field, or MemberIds Field, or Type Field");

            foreach (var newPreferredDay in newRequestedCourses.PreferredDays)
            {
                var requestedPreferredDay = new NewCoursePreferredDayRequest
                {
                    Day = newPreferredDay.Day,
                    FromTime = newPreferredDay.FromTime.ToTimeSpan(),
                    ToTime = newPreferredDay.ToTime.ToTimeSpan()
                };

                request.NewCoursePreferredDayRequests ??= new List<NewCoursePreferredDayRequest>();
                request.NewCoursePreferredDayRequests.Add(requestedPreferredDay);
            }

            if (newRequestedCourses.Courses == null || newRequestedCourses.Courses.Count == 0)
            {
                throw new BadRequestException("The courses field is required.");
            }

            var requestedCourses = newRequestedCourses.Courses.Select(c => c.Course).ToList();
            var existedCourses = await _context.Courses
                        .Include(c => c.Subjects)
                        .Include(c => c.Levels)
                        .Where(c => requestedCourses.Contains(c.course)).ToListAsync();

            foreach (var newRequestedCourse in newRequestedCourses.Courses)
            {
                var newRequestedCourseRequest = new NewCourseRequest()
                {
                    StudyCourseType = newRequestedCourses.Type,
                };
                var course = existedCourses.FirstOrDefault(c => c.course.ToLower() == newRequestedCourse.Course.ToLower());
                if (course == null)
                {
                    var newCourse = new Course
                    {
                        course = newRequestedCourse.Course
                    };

                    if (newRequestedCourse.Subjects == null)
                        throw new BadRequestException("The subjects field is required.");

                    foreach (var subject in newRequestedCourse.Subjects)
                    {
                        var newSubject = new Subject
                        {
                            subject = subject.Subject
                        };
                        newCourse.Subjects ??= new List<Subject>();
                        newCourse.Subjects.Add(newSubject);
                    }

                    if (newRequestedCourse.Level != null)
                    {
                        var newLevel = new Level
                        {
                            level = newRequestedCourse.Level
                        };
                        newCourse.Levels ??= new List<Level>();
                        newCourse.Levels.Add(newLevel);
                    }

                    _context.Courses.Add(newCourse);
                    await _context.SaveChangesAsync();
                    existedCourses = await _context.Courses
                        .Include(c => c.Subjects)
                        .Where(c => requestedCourses.Contains(c.course)).ToListAsync();

                    course = existedCourses.First(c => c.course == newRequestedCourse.Course);

                    var level = course.Levels.FirstOrDefault(c => c.level.ToLower() == newRequestedCourse.Level?.ToLower());
                    newRequestedCourseRequest.Level = level;

                    var requestedSubjects = newRequestedCourse.Subjects.Select(s => s.Subject).ToList();
                    var existedSubjects = course.Subjects.Where(s => requestedSubjects.Contains(s.subject));
                    foreach (var requestedSubject in newRequestedCourse.Subjects)
                    {
                        var subject = existedSubjects.First(s => s.subject.ToLower() == requestedSubject.Subject.ToLower());
                        var newRequestedSubject = new NewCourseSubjectRequest
                        {
                            Subject = subject,
                            Hour = requestedSubject.Hour
                        };

                        newRequestedCourseRequest.NewCourseSubjectRequests ??= new List<NewCourseSubjectRequest>();
                        newRequestedCourseRequest.NewCourseSubjectRequests.Add(newRequestedSubject);
                    }
                }
                else
                {
                    if (newRequestedCourse.Subjects == null)
                        throw new BadRequestException($"The subjects field is required for {newRequestedCourse.Course}");
                    foreach (var requestedSubject in newRequestedCourse.Subjects)
                    {
                        var newRequestedSubject = new NewCourseSubjectRequest();
                        var subject = course.Subjects.FirstOrDefault(s => s.subject == requestedSubject.Subject);
                        if (subject == null)
                        {
                            var newSubject = new Subject
                            {
                                subject = requestedSubject.Subject,
                                Course = course
                            };
                            _context.Subjects.Add(newSubject);
                            await _context.SaveChangesAsync();
                            existedCourses = await _context.Courses
                                                .Include(c => c.Subjects)
                                                .Where(c => requestedCourses
                                                .Contains(c.course)).ToListAsync();
                            course = existedCourses.First(c => c.course == course.course);

                            newRequestedSubject.Subject = newSubject;
                        }
                        else
                        {
                            newRequestedSubject.Subject = subject;
                        }
                        newRequestedSubject.Hour = requestedSubject.Hour;
                        newRequestedCourseRequest.NewCourseSubjectRequests ??= new List<NewCourseSubjectRequest>();
                        newRequestedCourseRequest.NewCourseSubjectRequests.Add(newRequestedSubject);
                    }

                    var level = course.Levels.FirstOrDefault(c => c.level == newRequestedCourse.Level);
                    if (newRequestedCourse.Level != null && level == null)
                    {
                        var newLevel = new Level
                        {
                            level = newRequestedCourse.Level,
                            Course = course
                        };
                        _context.Levels.Add(newLevel);
                        await _context.SaveChangesAsync();
                        course = await _context.Courses
                                            .Include(c => c.Subjects)
                                            .Include(c => c.Levels)
                                            .FirstAsync(c => c.course == newRequestedCourse.Course);
                        level = course.Levels.FirstOrDefault(c => c.level == newRequestedCourse.Level);
                    }
                    newRequestedCourseRequest.Level = level;
                }

                newRequestedCourseRequest.Course = course;
                newRequestedCourseRequest.TotalHours = newRequestedCourse.TotalHours;
                newRequestedCourseRequest.Method = newRequestedCourse.Method;
                newRequestedCourseRequest.StartDate = DateTime.Parse(newRequestedCourse.StartDate);
                newRequestedCourseRequest.EndDate = DateTime.Parse(newRequestedCourse.EndDate);
                request.NewCourseRequests ??= new List<NewCourseRequest>();
                request.NewCourseRequests.Add(newRequestedCourseRequest);
            }

            int byECId = _firebaseService.GetAzureIdWithToken();
            request.CreatedByStaffId = byECId;
            request.Type = RegistrationRequestType.NewRequestedCourse;
            request.RegistrationStatus = RegistrationStatus.PendingEA;

            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == byECId) ?? throw new NotFoundException($"Staff with ID {byECId} is not found.");

            foreach (var comment in newRequestedCourses.Comments)
            {
                var newComment = new RegistrationRequestComment
                {
                    Staff = staff,
                    Comment = comment
                };
                request.RegistrationRequestComments ??= new List<RegistrationRequestComment>();
                request.RegistrationRequestComments.Add(newComment);
            }

            var dbEAs = await _context.Staff
                        .Where(s => s.Role == "ea")
                        .ToListAsync();

            foreach (var ea in dbEAs)
            {
                var notification = new StaffNotification
                {
                    Staff = ea,
                    RegistrationRequest = request,
                    Title = "New Course Registration Request",
                    Message = "A new course registration request has been requested. Click here for more details.",
                    DateCreated = DateTime.Now,
                    Type = StaffNotificationType.RegistrationRequest,
                    HasRead = false
                };

                _context.StaffNotifications.Add(notification);
            }

            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<string>> AddStudentAddingRequest(StudentAddingRequestDto newRequest, List<IFormFile> filesToUpload)
        {
            var response = new ServiceResponse<string>();
            var request = new RegistrationRequest
            {
                PaymentByStaffId = _firebaseService.GetAzureIdWithToken()
            };

            if (newRequest.MemberIds == null || newRequest.MemberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }

            var dbStudents = new List<Student>();

            foreach (var memberId in newRequest.MemberIds)
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == memberId) ?? throw new NotFoundException($"Student with ID {memberId} not found.");

                if (dbStudent.Status == StudentStatus.Inactive)
                {
                    dbStudent.Status = StudentStatus.OnProcess;
                }

                dbStudents.Add(dbStudent);
                var member = new RegistrationRequestMember
                {
                    Student = dbStudent
                };
                request.RegistrationRequestMembers ??= new List<RegistrationRequestMember>();
                request.RegistrationRequestMembers.Add(member);
            }

            foreach (var studyCourse in newRequest.StudyCourse)
            {
                var dbStudyCourse = await _context.StudyCourses
                                                .Include(s => s.StudySubjects)
                                                    .ThenInclude(s => s.StudySubjectMember)
                                                .FirstOrDefaultAsync(s => s.Id == studyCourse.StudyCourseId) ?? throw new NotFoundException($"Study Course with ID {studyCourse.StudyCourseId} not found");

                var newStudentAddingRequest = new StudentAddingRequest
                {
                    StudyCourse = dbStudyCourse,
                    StudyCourseType = dbStudyCourse.StudyCourseType
                };

                foreach (var studySubjectId in studyCourse.StudySubjectIds)
                {
                    var dbStudySubject = dbStudyCourse.StudySubjects.FirstOrDefault(s => s.Id == studySubjectId) ?? throw new NotFoundException($"Study Subject with ID {studySubjectId} not found");

                    newStudentAddingRequest.StudentAddingSubjectRequests ??= new List<StudentAddingSubjectRequest>();
                    newStudentAddingRequest.StudentAddingSubjectRequests.Add(new StudentAddingSubjectRequest()
                    {
                        StudySubject = dbStudySubject
                    });

                    foreach (var dbStudent in dbStudents)
                    {
                        if (dbStudySubject.StudySubjectMember.Any(m => m.StudentId == dbStudent.Id))
                            throw new BadRequestException($"Student with code {dbStudent.StudentCode} is already enrolled this subject.");

                        dbStudySubject.StudySubjectMember ??= new List<StudySubjectMember>();
                        dbStudySubject.StudySubjectMember.Add(new StudySubjectMember()
                        {
                            Student = dbStudent,
                            Status = StudySubjectMemberStatus.Pending,
                        });
                    }
                }
                request.StudentAddingRequest ??= new List<StudentAddingRequest>();
                request.StudentAddingRequest.Add(newStudentAddingRequest);
            }

            int byECId = _firebaseService.GetAzureIdWithToken();
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == byECId) ?? throw new NotFoundException($"Staff with ID {byECId} is not found.");

            foreach (var comment in newRequest.Comments)
            {
                var newComment = new RegistrationRequestComment
                {
                    Staff = staff,
                    Comment = comment
                };
                request.RegistrationRequestComments ??= new List<RegistrationRequestComment>();
                request.RegistrationRequestComments.Add(newComment);
            }

            request.CreatedByStaffId = byECId;
            request.PaymentType = newRequest.PaymentType;
            request.RegistrationStatus = RegistrationStatus.PendingOA;
            request.Type = RegistrationRequestType.StudentAdding;
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            foreach (var newPaymentFile in filesToUpload)
            {
                var objectName = await _firebaseService.UploadRegistrationRequestPaymentFile(request.Id, request.DateCreated, newPaymentFile);
                request.RegistrationRequestPaymentFiles ??= new List<RegistrationRequestPaymentFile>();
                if (!request.RegistrationRequestPaymentFiles.Any(f => f.ObjectName == objectName))
                {
                    request.RegistrationRequestPaymentFiles.Add(new RegistrationRequestPaymentFile()
                    {
                        FileName = newPaymentFile.FileName,
                        ObjectName = objectName,
                    });
                }
            }

            var dbOAs = await _context.Staff
            .Where(s => s.Role == "oa")
            .ToListAsync();

            foreach (var oa in dbOAs)
            {
                var notification = new StaffNotification
                {
                    Staff = oa,
                    RegistrationRequest = request,
                    Title = "New Student Adding Payment Approval Request",
                    Message = "A new student adding payment approval request has been requested. Click here for more details.",
                    DateCreated = DateTime.Now,
                    Type = StaffNotificationType.RegistrationRequest,
                    HasRead = false
                };

                _context.StaffNotifications.Add(notification);
            }

            foreach (var dbStudent in dbStudents)
            {
                var allStudyClasses = await _context.StudyClasses
                                            .Include(x => x.Schedule)
                                            .Where(sc => sc.StudySubject.StudySubjectMember.Any(sm => sm.StudentId == dbStudent.Id)
                                                      && sc.StudyCourse.Status == StudyCourseStatus.Ongoing)
                                            .ToListAsync();

                var lastClassEndDate = allStudyClasses.Max(sc => sc.Schedule.Date);

                var expiryDate = lastClassEndDate.AddDays(14);

                dbStudent.ExpiryDate = expiryDate;
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK; ;
            return response;
        }

        public async Task<ServiceResponse<List<RegistrationRequestResponseDto>>> ListRegistrationRequests()
        {
            var response = new ServiceResponse<List<RegistrationRequestResponseDto>>();
            var registrationRequests = await _context.RegistrationRequests
                    .Include(r => r.RegistrationRequestMembers)
                        .ThenInclude(m => m.Student)
                    .Include(r => r.NewCourseRequests)
                    .Include(r => r.StudentAddingRequest)
                        .ThenInclude(s => s.StudyCourse)
                    .ToListAsync();

            var data = new List<RegistrationRequestResponseDto>();

            var dbStaff = await _context.Staff.ToListAsync();
            foreach (var registrationRequest in registrationRequests)
            {
                var requestDto = new RegistrationRequestResponseDto();

                foreach (var student in registrationRequest.RegistrationRequestMembers)
                {
                    var studentDto = new StudentNameResponseDto
                    {
                        StudentId = student.Student.Id,
                        StudentCode = student.Student.StudentCode!,
                        FirstName = student.Student.FirstName,
                        LastName = student.Student.LastName,
                        FullName = student.Student.FullName,
                        Nickname = student.Student.Nickname
                    };
                    requestDto.Members.Add(studentDto);
                }

                if (registrationRequest.Type == RegistrationRequestType.NewRequestedCourse)
                {
                    foreach (var dbNewCourse in registrationRequest.NewCourseRequests)
                    {
                        if (!requestDto.StudyCourseType.Any(t => t == dbNewCourse.StudyCourseType))
                            requestDto.StudyCourseType.Add(dbNewCourse.StudyCourseType);
                    }
                    requestDto.Section = registrationRequest.Section;
                }
                else
                {
                    var section = registrationRequest.StudentAddingRequest.ElementAt(0).StudyCourse.Section;
                    foreach (var dbStudentAdding in registrationRequest.StudentAddingRequest)
                    {
                        if (!requestDto.StudyCourseType.Any(t => t == dbStudentAdding.StudyCourseType))
                            requestDto.StudyCourseType.Add(dbStudentAdding.StudyCourseType);
                        if (dbStudentAdding.StudyCourse.Section != section)
                            section = "Multiple";
                    }
                    requestDto.Section = section;
                }

                requestDto.RequestId = registrationRequest.Id;
                requestDto.Type = registrationRequest.Type;
                requestDto.RegistrationStatus = registrationRequest.RegistrationStatus;
                requestDto.PaymentType = registrationRequest.PaymentType;
                requestDto.PaymentStatus = registrationRequest.PaymentStatus;
                requestDto.CreatedDate = registrationRequest.DateCreated;
                requestDto.PaymentError = registrationRequest.PaymentError;
                requestDto.ScheduleError = registrationRequest.ScheduleError;
                requestDto.NewCourseDetailError = registrationRequest.NewCourseDetailError;
                requestDto.HasSchedule = registrationRequest.HasSchedule;

                var ec = dbStaff.FirstOrDefault(s => s.Id == registrationRequest.CreatedByStaffId);
                var scheduledBy = dbStaff.FirstOrDefault(s => s.Id == registrationRequest.ScheduledByStaffId);
                var takenBy = dbStaff.FirstOrDefault(s => s.Id == registrationRequest.TakenByEAId);
                var oa = dbStaff.FirstOrDefault(s => s.Id == registrationRequest.ReviewedByStaffId);
                var cancelledBy = dbStaff.FirstOrDefault(s => s.Id == registrationRequest.CancelledBy);

                if (ec != null)
                {
                    var staff = new StaffNameOnlyResponseDto
                    {
                        StaffId = ec.Id,
                        Nickname = ec.Nickname,
                        FullName = ec.FullName
                    };
                    requestDto.ByEC = staff;
                }
                if (takenBy != null)
                {
                    var staff = new StaffNameOnlyResponseDto
                    {
                        StaffId = takenBy.Id,
                        Nickname = takenBy.Nickname,
                        FullName = takenBy.FullName
                    };

                    requestDto.TakenByEA = staff;
                }
                if (scheduledBy != null)
                {
                    var staff = new StaffNameOnlyResponseDto
                    {
                        StaffId = scheduledBy.Id,
                        Nickname = scheduledBy.Nickname,
                        FullName = scheduledBy.FullName
                    };
                    requestDto.ScheduledBy = staff;
                }
                if (oa != null)
                {
                    var staff = new StaffNameOnlyResponseDto
                    {
                        StaffId = oa.Id,
                        Nickname = oa.Nickname,
                        FullName = oa.FullName
                    };
                    requestDto.ByOA = staff;
                }
                if (cancelledBy != null)
                {
                    var staff = new StaffNameOnlyResponseDto
                    {
                        StaffId = cancelledBy.Id,
                        Nickname = cancelledBy.Nickname,
                        FullName = cancelledBy.FullName
                    };
                    requestDto.CancelledBy = staff;
                }

                data.Add(requestDto);

            }
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<EcRegistrationRequestDetailResponseDto>> EcGetRequestDetail(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == requestId)
                                                                ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            var requestDetail = new EcRegistrationRequestDetailResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                PaymentType = dbRequest.PaymentType,
                PaymentStatus = dbRequest.PaymentStatus,
                PaymentError = dbRequest.PaymentError,
                ScheduleError = dbRequest.ScheduleError,
                NewCourseDetailError = dbRequest.NewCourseDetailError,
                HasSchedule = dbRequest.HasSchedule,
            };
            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                if (dbRequest.HasSchedule == false)
                {
                    dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                    foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                    {
                        var requestedCourse = new RequestedCourseResponseDto()
                        {
                            Section = dbRequest.Section,
                            CourseId = dbRequestedCourse.Course.Id,
                            Course = dbRequestedCourse.Course.course,
                            LevelId = dbRequestedCourse.LevelId,
                            Level = dbRequestedCourse.Level?.level,
                            TotalHours = dbRequestedCourse.TotalHours,
                            StartDate = dbRequestedCourse.StartDate.ToDateString(),
                            EndDate = dbRequestedCourse.EndDate.ToDateString(),
                            Method = dbRequestedCourse.Method,
                            StudyCourseType = dbRequestedCourse.StudyCourseType,
                        };
                        foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                        {
                            var requestSubject = new RequestedSubjectResponseDto()
                            {
                                StudySubjectId = dbRequestSubject.Id,
                                SubjectId = dbRequestSubject.Subject.Id,
                                Subject = dbRequestSubject.Subject.subject,
                                Hour = dbRequestSubject.Hour,
                            };
                            requestedCourse.subjects.Add(requestSubject);
                        }
                        requestDetail.Courses.Add(requestedCourse);
                    }
                }
                else
                {
                    dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                    foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                    {
                        var requestedCourse = new RequestedCourseResponseDto()
                        {
                            Section = dbRequestedCourse.StudyCourse?.Section,
                            StudyCourseId = dbRequestedCourse.StudyCourse?.Id,
                            CourseId = dbRequestedCourse.Course.Id,
                            Course = dbRequestedCourse.Course.course,
                            LevelId = dbRequestedCourse.LevelId,
                            Level = dbRequestedCourse.Level?.level,
                            TotalHours = dbRequestedCourse.TotalHours,
                            StartDate = dbRequestedCourse.StartDate.ToDateString(),
                            EndDate = dbRequestedCourse.EndDate.ToDateString(),
                            Method = dbRequestedCourse.Method,
                            StudyCourseType = dbRequestedCourse.StudyCourseType,
                        };

                        foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                        {
                            var requestSubject = new RequestedSubjectResponseDto()
                            {
                                StudySubjectId = dbRequestSubject.Id,
                                SubjectId = dbRequestSubject.Subject.Id,
                                Subject = dbRequestSubject.Subject.subject,
                                Hour = dbRequestSubject.Hour,
                            };
                            requestedCourse.subjects.Add(requestSubject);
                        }
                        requestDetail.Courses.Add(requestedCourse);
                    }
                    requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
                }

                requestDetail.StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType;
            }
            else
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(c => c.Subject)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Level)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.StudentAdding);

                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        StudyCourseId = dbStudentAddingRequest.StudyCourse.Id,
                        Section = dbStudentAddingRequest.StudyCourse?.Section,
                        CourseId = dbStudentAddingRequest.StudyCourse!.Course.Id,
                        Course = dbStudentAddingRequest.StudyCourse.Course.course,
                        LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                        Level = dbStudentAddingRequest.StudyCourse.Level?.level,
                        TotalHours = dbStudentAddingRequest.StudyCourse.TotalHour,
                        StartDate = dbStudentAddingRequest.StudyCourse.StartDate.ToDateString(),
                        EndDate = dbStudentAddingRequest.StudyCourse.EndDate.ToDateString(),
                        Method = dbStudentAddingRequest.StudyCourse.Method,
                        StudyCourseType = dbStudentAddingRequest.StudyCourseType,
                    };
                    foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = StudentAddingRequestMapScheduleDto(dbRequest.StudentAddingRequest);
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var objectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);

                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url
                });
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<EcRegistrationRequestDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = requestDetail,
            };
            return response;
        }

        public async Task<ServiceResponse<EaRegistrationRequestDetailResponseDto>> EaGetRequestDetail(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == requestId)
                                                                ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            var requestDetail = new EaRegistrationRequestDetailResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                PaymentType = dbRequest.PaymentType,
                PaymentStatus = dbRequest.PaymentStatus,
                PaymentError = dbRequest.PaymentError,
                ScheduleError = dbRequest.ScheduleError,
                NewCourseDetailError = dbRequest.NewCourseDetailError,
                HasSchedule = dbRequest.HasSchedule,
            };
            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                if (dbRequest.HasSchedule == false)
                {
                    dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                    foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                    {
                        var requestedCourse = new RequestedCourseResponseDto()
                        {
                            Section = dbRequest.Section,
                            CourseId = dbRequestedCourse.Course.Id,
                            Course = dbRequestedCourse.Course.course,
                            LevelId = dbRequestedCourse.LevelId,
                            Level = dbRequestedCourse.Level?.level,
                            TotalHours = dbRequestedCourse.TotalHours,
                            StartDate = dbRequestedCourse.StartDate.ToDateString(),
                            EndDate = dbRequestedCourse.EndDate.ToDateString(),
                            Method = dbRequestedCourse.Method,
                            StudyCourseType = dbRequestedCourse.StudyCourseType,
                        };
                        foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                        {
                            var requestSubject = new RequestedSubjectResponseDto()
                            {
                                StudySubjectId = dbRequestSubject.Id,
                                SubjectId = dbRequestSubject.Subject.Id,
                                Subject = dbRequestSubject.Subject.subject,
                                Hour = dbRequestSubject.Hour,
                            };
                            requestedCourse.subjects.Add(requestSubject);
                        }
                        requestDetail.Courses.Add(requestedCourse);
                    }
                }
                else
                {
                    dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                    foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                    {
                        var requestedCourse = new RequestedCourseResponseDto()
                        {
                            Section = dbRequestedCourse.StudyCourse?.Section,
                            StudyCourseId = dbRequestedCourse.StudyCourse?.Id,
                            CourseId = dbRequestedCourse.Course.Id,
                            Course = dbRequestedCourse.Course.course,
                            LevelId = dbRequestedCourse.LevelId,
                            Level = dbRequestedCourse.Level?.level,
                            TotalHours = dbRequestedCourse.TotalHours,
                            StartDate = dbRequestedCourse.StartDate.ToDateString(),
                            EndDate = dbRequestedCourse.EndDate.ToDateString(),
                            Method = dbRequestedCourse.Method,
                            StudyCourseType = dbRequestedCourse.StudyCourseType,
                        };

                        foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                        {
                            var requestSubject = new RequestedSubjectResponseDto()
                            {
                                StudySubjectId = dbRequestSubject.Id,
                                SubjectId = dbRequestSubject.Subject.Id,
                                Subject = dbRequestSubject.Subject.subject,
                                Hour = dbRequestSubject.Hour,
                            };
                            requestedCourse.subjects.Add(requestSubject);
                        }
                        requestDetail.Courses.Add(requestedCourse);
                    }
                    requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
                }

                requestDetail.StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType;
            }
            else
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(c => c.Subject)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Level)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.StudentAdding);

                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        StudyCourseId = dbStudentAddingRequest.StudyCourse.Id,
                        Section = dbStudentAddingRequest.StudyCourse?.Section,
                        CourseId = dbStudentAddingRequest.StudyCourse!.Course.Id,
                        Course = dbStudentAddingRequest.StudyCourse.Course.course,
                        LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                        Level = dbStudentAddingRequest.StudyCourse.Level?.level,
                        TotalHours = dbStudentAddingRequest.StudyCourse.TotalHour,
                        StartDate = dbStudentAddingRequest.StudyCourse.StartDate.ToDateString(),
                        EndDate = dbStudentAddingRequest.StudyCourse.EndDate.ToDateString(),
                        Method = dbStudentAddingRequest.StudyCourse.Method,
                        StudyCourseType = dbStudentAddingRequest.StudyCourseType,
                    };
                    foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = StudentAddingRequestMapScheduleDto(dbRequest.StudentAddingRequest);
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var objectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);

                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url
                });
            }

            var takenByEADetail = new StaffNameOnlyResponseDto();
            var ea = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.TakenByEAId);
            if (ea != null)
            {
                takenByEADetail.StaffId = ea.Id;
                takenByEADetail.FullName = ea.FullName;
                takenByEADetail.Nickname = ea.Nickname;
                requestDetail.TakenByEA = takenByEADetail;
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<EaRegistrationRequestDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = requestDetail,
            };
            return response;
        }

        public async Task<ServiceResponse<OaRegistrationRequestDetailResponseDto>> OaGetRequestDetail(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == requestId)
                                                                ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            var requestDetail = new OaRegistrationRequestDetailResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                PaymentType = dbRequest.PaymentType,
                PaymentStatus = dbRequest.PaymentStatus,
                PaymentError = dbRequest.PaymentError,
                ScheduleError = dbRequest.ScheduleError,
                NewCourseDetailError = dbRequest.NewCourseDetailError,
                HasSchedule = dbRequest.HasSchedule,
            };
            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                if (dbRequest.HasSchedule == false)
                {
                    dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                    foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                    {
                        var requestedCourse = new RequestedCourseResponseDto()
                        {
                            Section = dbRequest.Section,
                            CourseId = dbRequestedCourse.Course.Id,
                            Course = dbRequestedCourse.Course.course,
                            LevelId = dbRequestedCourse.LevelId,
                            Level = dbRequestedCourse.Level?.level,
                            TotalHours = dbRequestedCourse.TotalHours,
                            StartDate = dbRequestedCourse.StartDate.ToDateString(),
                            EndDate = dbRequestedCourse.EndDate.ToDateString(),
                            Method = dbRequestedCourse.Method,
                            StudyCourseType = dbRequestedCourse.StudyCourseType,
                        };
                        foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                        {
                            var requestSubject = new RequestedSubjectResponseDto()
                            {
                                StudySubjectId = dbRequestSubject.Id,
                                SubjectId = dbRequestSubject.Subject.Id,
                                Subject = dbRequestSubject.Subject.subject,
                                Hour = dbRequestSubject.Hour,
                            };
                            requestedCourse.subjects.Add(requestSubject);
                        }
                        requestDetail.Courses.Add(requestedCourse);
                    }
                }
                else
                {
                    dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                    foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                    {
                        var requestedCourse = new RequestedCourseResponseDto()
                        {
                            Section = dbRequestedCourse.StudyCourse?.Section,
                            StudyCourseId = dbRequestedCourse.StudyCourse?.Id,
                            CourseId = dbRequestedCourse.Course.Id,
                            Course = dbRequestedCourse.Course.course,
                            LevelId = dbRequestedCourse.LevelId,
                            Level = dbRequestedCourse.Level?.level,
                            TotalHours = dbRequestedCourse.TotalHours,
                            StartDate = dbRequestedCourse.StartDate.ToDateString(),
                            EndDate = dbRequestedCourse.EndDate.ToDateString(),
                            Method = dbRequestedCourse.Method,
                            StudyCourseType = dbRequestedCourse.StudyCourseType,
                        };

                        foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                        {
                            var requestSubject = new RequestedSubjectResponseDto()
                            {
                                StudySubjectId = dbRequestSubject.Id,
                                SubjectId = dbRequestSubject.Subject.Id,
                                Subject = dbRequestSubject.Subject.subject,
                                Hour = dbRequestSubject.Hour,
                            };
                            requestedCourse.subjects.Add(requestSubject);
                        }
                        requestDetail.Courses.Add(requestedCourse);
                    }
                    requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
                }

                requestDetail.StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType;
            }
            else
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(c => c.Subject)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Level)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId
                            && r.Type == RegistrationRequestType.StudentAdding);

                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        StudyCourseId = dbStudentAddingRequest.StudyCourse.Id,
                        Section = dbStudentAddingRequest.StudyCourse?.Section,
                        CourseId = dbStudentAddingRequest.StudyCourse!.Course.Id,
                        Course = dbStudentAddingRequest.StudyCourse.Course.course,
                        LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                        Level = dbStudentAddingRequest.StudyCourse.Level?.level,
                        TotalHours = dbStudentAddingRequest.StudyCourse.TotalHour,
                        StartDate = dbStudentAddingRequest.StudyCourse.StartDate.ToDateString(),
                        EndDate = dbStudentAddingRequest.StudyCourse.EndDate.ToDateString(),
                        Method = dbStudentAddingRequest.StudyCourse.Method,
                        StudyCourseType = dbStudentAddingRequest.StudyCourseType,
                    };
                    foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = StudentAddingRequestMapScheduleDto(dbRequest.StudentAddingRequest);
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var objectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);

                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url
                });
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<OaRegistrationRequestDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = requestDetail,
            };
            return response;
        }

        public async Task<ServiceResponse<RegistrationRequestPendingEADetailResponseDto>> GetPendingEADetail(int requestId)
        {
            var response = new ServiceResponse<RegistrationRequestPendingEADetailResponseDto>();
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.Type == RegistrationRequestType.NewRequestedCourse
                                                && r.RegistrationStatus == RegistrationStatus.PendingEA) ?? throw new NotFoundException($"Pending EA Request with ID {requestId} is not found.");

            var ea = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.TakenByEAId);

            var requestDetail = new RegistrationRequestPendingEADetailResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType,
            };

            var takenByEADetail = new StaffNameOnlyResponseDto();

            if (ea != null)
            {
                takenByEADetail.StaffId = ea.Id;
                takenByEADetail.FullName = ea.FullName;
                takenByEADetail.Nickname = ea.Nickname;
                requestDetail.TakenByEA = takenByEADetail;
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }

            foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
            {
                var requestedCourse = new RequestedCourseResponseDto()
                {
                    CourseId = dbRequestedCourse.Course.Id,
                    Course = dbRequestedCourse.Course.course,
                    LevelId = dbRequestedCourse.LevelId,
                    Level = dbRequestedCourse.Level?.level,
                    TotalHours = dbRequestedCourse.TotalHours,
                    StartDate = dbRequestedCourse.StartDate.ToDateString(),
                    EndDate = dbRequestedCourse.EndDate.ToDateString(),
                    Method = dbRequestedCourse.Method,
                    StudyCourseType = dbRequestedCourse.StudyCourseType,
                };
                foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                {
                    var requestSubject = new RequestedSubjectResponseDto()
                    {
                        SubjectId = dbRequestSubject.Subject.Id,
                        Subject = dbRequestSubject.Subject.subject,
                        Hour = dbRequestSubject.Hour,
                    };
                    requestedCourse.subjects.Add(requestSubject);
                }
                requestDetail.Courses.Add(requestedCourse);
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToString("dd-MMMM-yyyy HH:mm:ss");
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }

            response.StatusCode = (int)HttpStatusCode.OK; ;
            response.Data = requestDetail;

            return response;
        }

        public async Task<ServiceResponse<string>> DeclineSchedule(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEC)
                            ?? throw new NotFoundException($"Pending EC with Request ID {requestId} is not found.");

            dbRequest.RegistrationStatus = RegistrationStatus.PendingEA;
            dbRequest.ScheduleError = true;

            var ea = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Id == dbRequest.ScheduledByStaffId)
                    ?? throw new NotFoundException("EA not found.");

            var eaNotification = new StaffNotification
            {
                Staff = ea,
                RegistrationRequest = dbRequest,
                Title = "Schedule Declined",
                Message = "The schedule has been declined. Click here for more details.",
                DateCreated = DateTime.Now,
                Type = StaffNotificationType.RegistrationRequest,
                HasRead = false
            };

            _context.StaffNotifications.Add(eaNotification);

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<RegistrationRequestPendingEADetail2ResponseDto>> GetPendingEADetail2(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                        .Include(r => r.NewCourseRequests)
                            .ThenInclude(c => c.NewCourseSubjectRequests)
                                .ThenInclude(s => s.Subject)
                        .Include(r => r.NewCourseRequests)
                            .ThenInclude(c => c.Course)
                        .Include(r => r.NewCourseRequests)
                            .ThenInclude(c => c.Level)
                        .Include(r => r.NewCourseRequests)
                            .ThenInclude(c => c.StudyCourse)
                                .ThenInclude(c => c!.StudySubjects)
                                    .ThenInclude(s => s.StudyClasses)
                                        .ThenInclude(c => c.Schedule)
                        .Include(r => r.NewCourseRequests)
                            .ThenInclude(c => c.StudyCourse)
                                .ThenInclude(c => c!.StudySubjects)
                                    .ThenInclude(s => s.StudyClasses)
                                        .ThenInclude(c => c.Teacher)
                        .Include(r => r.NewCourseRequests)
                            .ThenInclude(c => c.StudyCourse)
                                .ThenInclude(c => c!.StudySubjects)
                                    .ThenInclude(s => s.StudyClasses)
                                        .ThenInclude(c => c.TeacherShifts)
                        .Include(r => r.RegistrationRequestMembers)
                            .ThenInclude(m => m.Student)
                        .Include(r => r.NewCoursePreferredDayRequests)
                        .Include(r => r.RegistrationRequestPaymentFiles)
                        .Include(r => r.RegistrationRequestComments)
                        .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEA
                        && r.Type == RegistrationRequestType.NewRequestedCourse)
                        ?? throw new NotFoundException($"Pending EA Request with ID {requestId} is not found.");

            var ea = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.TakenByEAId);

            var requestDetail = new RegistrationRequestPendingEADetail2ResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType,
            };

            var takenByEADetail = new StaffNameOnlyResponseDto();

            if (ea != null)
            {
                takenByEADetail.StaffId = ea.Id;
                takenByEADetail.FullName = ea.FullName;
                takenByEADetail.Nickname = ea.Nickname;
                requestDetail.TakenByEA = takenByEADetail;
            }

            foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
            {
                var requestedCourse = new RequestedCourseResponseDto()
                {
                    Section = dbRequestedCourse.StudyCourse?.Section,
                    CourseId = dbRequestedCourse.Course.Id,
                    Course = dbRequestedCourse.Course.course,
                    StudyCourseId = dbRequestedCourse.StudyCourse?.Id,
                    LevelId = dbRequestedCourse.LevelId,
                    Level = dbRequestedCourse.Level?.level,
                    TotalHours = dbRequestedCourse.TotalHours,
                    StartDate = dbRequestedCourse.StartDate.ToDateString(),
                    EndDate = dbRequestedCourse.EndDate.ToDateString(),
                    Method = dbRequestedCourse.Method,
                    StudyCourseType = dbRequestedCourse.StudyCourseType,
                };

                foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                {
                    var requestSubject = new RequestedSubjectResponseDto()
                    {
                        SubjectId = dbRequestSubject.Subject.Id,
                        Subject = dbRequestSubject.Subject.subject,
                        Hour = dbRequestSubject.Hour,
                        StudySubjectId = dbRequestedCourse.StudyCourse?.StudySubjects.FirstOrDefault(s => s.SubjectId == dbRequestSubject.SubjectId)?.Id,
                    };
                    requestedCourse.subjects.Add(requestSubject);
                }
                requestDetail.Courses.Add(requestedCourse);
            }
            requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<RegistrationRequestPendingEADetail2ResponseDto>
            {
                Data = requestDetail,
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<RegistrationRequestPendingECResponseDto>> GetPendingECDetail(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEC)
                                                                ?? throw new NotFoundException($"Pending EC Request with ID {requestId} is not found.");

            var requestDetail = new RegistrationRequestPendingECResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                PaymentType = dbRequest.PaymentType,
            };
            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEC
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                requestDetail.StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType;

                foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        Section = dbRequestedCourse.StudyCourse?.Section,
                        StudyCourseId = dbRequestedCourse.StudyCourse?.Id,
                        CourseId = dbRequestedCourse.Course.Id,
                        Course = dbRequestedCourse.Course.course,
                        LevelId = dbRequestedCourse.LevelId,
                        Level = dbRequestedCourse.Level?.level,
                        TotalHours = dbRequestedCourse.TotalHours,
                        StartDate = dbRequestedCourse.StartDate.ToDateString(),
                        EndDate = dbRequestedCourse.EndDate.ToDateString(),
                        Method = dbRequestedCourse.Method,
                        StudyCourseType = dbRequestedCourse.StudyCourseType,
                    };

                    foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            StudySubjectId = dbRequestSubject.Id,
                            SubjectId = dbRequestSubject.Subject.Id,
                            Subject = dbRequestSubject.Subject.subject,
                            Hour = dbRequestSubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
            }
            else
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(c => c.Subject)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Level)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEC
                            && r.Type == RegistrationRequestType.StudentAdding);

                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        StudyCourseId = dbStudentAddingRequest.StudyCourse.Id,
                        Section = dbStudentAddingRequest.StudyCourse?.Section,
                        CourseId = dbStudentAddingRequest.StudyCourse!.Course.Id,
                        Course = dbStudentAddingRequest.StudyCourse.Course.course,
                        LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                        Level = dbStudentAddingRequest.StudyCourse.Level?.level,
                        TotalHours = dbStudentAddingRequest.StudyCourse.TotalHour,
                        StartDate = dbStudentAddingRequest.StudyCourse.StartDate.ToDateString(),
                        EndDate = dbStudentAddingRequest.StudyCourse.EndDate.ToDateString(),
                        Method = dbStudentAddingRequest.StudyCourse.Method,
                        StudyCourseType = dbStudentAddingRequest.StudyCourseType,
                    };
                    foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = StudentAddingRequestMapScheduleDto(dbRequest.StudentAddingRequest);
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var objectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);

                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url
                });
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<RegistrationRequestPendingECResponseDto>
            {
                Data = requestDetail,
                StatusCode = (int)HttpStatusCode.OK
            };
            return response;
        }

        public async Task<ServiceResponse<String>> SubmitPayment(int requestId, SubmitPaymentRequestDto paymentRequest, List<IFormFile> filesToUpload)
        {
            var dbRequest = await _context.RegistrationRequests
                                    .Include(r => r.RegistrationRequestPaymentFiles)
                                    .FirstOrDefaultAsync(
                                    r => r.Id == requestId
                                    && r.RegistrationStatus == RegistrationStatus.PendingEC) ?? throw new NotFoundException($"Pending EC request with ID {requestId} is not found.");

            var removeFiles = dbRequest.RegistrationRequestPaymentFiles
                                        .Where(f => paymentRequest.FilesToDelete
                                        .Contains(f.FileName))
                                        .ToList();
            foreach (var file in removeFiles)
            {
                await _firebaseService.DeleteStorageFileByObjectName(file.ObjectName);
                dbRequest.RegistrationRequestPaymentFiles.Remove(file);
            }

            foreach (var newPaymentFile in filesToUpload)
            {
                var objectName = await _firebaseService.UploadRegistrationRequestPaymentFile(requestId, dbRequest.DateCreated, newPaymentFile);
                if (!dbRequest.RegistrationRequestPaymentFiles.Any(f => f.ObjectName == objectName))
                {
                    dbRequest.RegistrationRequestPaymentFiles.Add(new RegistrationRequestPaymentFile()
                    {
                        FileName = newPaymentFile.FileName,
                        ObjectName = objectName,
                    });
                }
            }
            dbRequest.RegistrationStatus = RegistrationStatus.PendingOA;
            dbRequest.PaymentType = paymentRequest.PaymentType;
            dbRequest.PaymentByStaffId = _firebaseService.GetAzureIdWithToken();

            var dbOAs = await _context.Staff
                        .Where(s => s.Role == "oa")
                        .ToListAsync();

            foreach (var oa in dbOAs)
            {
                var notification = new StaffNotification
                {
                    Staff = oa,
                    RegistrationRequest = dbRequest,
                    Title = "New Registration Request Payment Approval Request",
                    Message = "A new payment approval request for a registration request has been requested. Click here for more details.",
                    DateCreated = DateTime.Now,
                    Type = StaffNotificationType.RegistrationRequest,
                    HasRead = false
                };

                _context.StaffNotifications.Add(notification);
            }

            await _context.SaveChangesAsync();


            var response = new ServiceResponse<String> { StatusCode = (int)HttpStatusCode.OK };
            return response;
        }

        public async Task<ServiceResponse<RegistrationRequestPendingOAResponseDto>> GetPendingOADetail(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingOA)
                                                                ?? throw new NotFoundException($"Pending OA Request with ID {requestId} is not found.");

            var requestDetail = new RegistrationRequestPendingOAResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                PaymentType = dbRequest.PaymentType
            };

            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingOA
                            && r.Type == RegistrationRequestType.NewRequestedCourse);

                requestDetail.StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType;

                foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        Section = dbRequestedCourse.StudyCourse?.Section,
                        CourseId = dbRequestedCourse.Course.Id,
                        Course = dbRequestedCourse.Course.course,
                        LevelId = dbRequestedCourse.LevelId,
                        Level = dbRequestedCourse.Level?.level,
                        TotalHours = dbRequestedCourse.TotalHours,
                        StartDate = dbRequestedCourse.StartDate.ToDateString(),
                        EndDate = dbRequestedCourse.EndDate.ToDateString(),
                        Method = dbRequestedCourse.Method,
                        StudyCourseType = dbRequestedCourse.StudyCourseType,
                    };
                    foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            SubjectId = dbRequestSubject.Subject.Id,
                            Subject = dbRequestSubject.Subject.subject,
                            Hour = dbRequestSubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
            }
            else
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Level)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(c => c.Subject)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingOA
                            && r.Type == RegistrationRequestType.StudentAdding);

                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        Section = dbStudentAddingRequest.StudyCourse?.Section,
                        StudyCourseId = dbStudentAddingRequest.StudyCourse?.Id,
                        CourseId = dbStudentAddingRequest.StudyCourse!.Course.Id,
                        Course = dbStudentAddingRequest.StudyCourse.Course.course,
                        LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                        Level = dbStudentAddingRequest.StudyCourse.Level?.level,
                        TotalHours = dbStudentAddingRequest.StudyCourse.TotalHour,
                        StartDate = dbStudentAddingRequest.StudyCourse.StartDate.ToDateString(),
                        EndDate = dbStudentAddingRequest.StudyCourse.EndDate.ToDateString(),
                        Method = dbStudentAddingRequest.StudyCourse.Method,
                        StudyCourseType = dbStudentAddingRequest.StudyCourseType,
                    };
                    foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            StudySubjectId = dbStudySubject.Id,
                            Hour = dbStudySubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = StudentAddingRequestMapScheduleDto(dbRequest.StudentAddingRequest);
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var objectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);

                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url
                });
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<RegistrationRequestPendingOAResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = requestDetail
            };
            return response;
        }

        public async Task<ServiceResponse<string>> ApprovePayment(int requestId, PaymentStatus paymentStatus)
        {
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s!.StudySubjects)
                                        .ThenInclude(s => s.StudySubjectMember)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Attendances)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s!.StudySubjects)
                                        .ThenInclude(s => s.StudySubjectMember)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Attendances)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingOA)
                            ?? throw new NotFoundException($"PendingOA Request with ID {requestId} is not found");

            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                foreach (var dbNewCourseRequest in dbRequest.NewCourseRequests)
                {
                    if (dbNewCourseRequest.StudyCourse != null)
                    {
                        dbNewCourseRequest.StudyCourse.Status = StudyCourseStatus.NotStarted;
                        foreach (var dbStudySubject in dbNewCourseRequest.StudyCourse.StudySubjects)
                        {
                            foreach (var dbStudySubjectMember in dbStudySubject.StudySubjectMember)
                            {
                                dbStudySubjectMember.Status = StudySubjectMemberStatus.Success;
                                dbStudySubjectMember.CourseJoinedDate = DateTime.Now;
                            }

                            foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                            {
                                foreach (var dbStudySubjectMember in dbStudySubject.StudySubjectMember)
                                {
                                    var attendance = new StudentAttendance
                                    {
                                        Student = dbStudySubjectMember.Student,
                                        StudyClass = dbStudyClass,
                                        Attendance = Attendance.None
                                    };

                                    _context.StudentAttendances.Add(attendance);

                                    var studentNotification = new StudentNotification
                                    {
                                        Student = dbStudySubjectMember.Student,
                                        StudyCourse = dbNewCourseRequest.StudyCourse,
                                        Title = "Registration Request Approval",
                                        Message = "The registration request has been approved. Click here for more details.",
                                        Type = StudentNotificationType.RegistrationRequest,
                                        DateCreated = DateTime.Now,
                                        HasRead = false
                                    };

                                    _context.StudentNotifications.Add(studentNotification);
                                }
                            }
                        }

                        var teachersInCourse = dbNewCourseRequest.StudyCourse.StudySubjects.SelectMany(ss => ss.StudyClasses).Select(sc => sc.Teacher).Distinct().ToList();

                        foreach (var teacher in teachersInCourse)
                        {
                            var teacherNotification = new TeacherNotification
                            {
                                Teacher = teacher,
                                StudyCourse = dbNewCourseRequest.StudyCourse,
                                Title = "New Course Assigned",
                                Message = "You have been assigned to a new course. Click here for more details.",
                                DateCreated = DateTime.Now,
                                Type = TeacherNotificationType.NewCourse,
                                HasRead = false
                            };

                            _context.TeacherNotifications.Add(teacherNotification);
                        }
                    }

                    if (dbNewCourseRequest.StudyCourse == null)
                        throw new InternalServerException("Something went wrong with NewCourseRequest and StudyCourse");
                }
            }
            else
            {
                var studentIdList = dbRequest.RegistrationRequestMembers.Select(s => s.StudentId).ToList();
                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    if (dbStudentAddingRequest.StudyCourse != null)
                    {
                        foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                        {
                            foreach (var dbStudySubjectMember in dbStudySubject.StudySubjectMember.Where(s => studentIdList.Contains(s.StudentId)))
                            {
                                dbStudySubjectMember.Status = StudySubjectMemberStatus.Success;
                            }

                            foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                            {
                                foreach (var dbStudySubjectMember in dbStudySubject.StudySubjectMember.Where(s => studentIdList.Contains(s.StudentId)))
                                {
                                    var attendance = new StudentAttendance
                                    {
                                        Student = dbStudySubjectMember.Student,
                                        StudyClass = dbStudyClass,
                                        Attendance = Attendance.None
                                    };

                                    _context.StudentAttendances.Add(attendance);

                                    var studentNotification = new StudentNotification
                                    {
                                        Student = dbStudySubjectMember.Student,
                                        StudyCourse = dbStudentAddingRequest.StudyCourse,
                                        Title = "Registration Request Approval",
                                        Message = "The registration request has been approved. Click here for more details.",
                                        Type = StudentNotificationType.RegistrationRequest,
                                        DateCreated = DateTime.Now,
                                        HasRead = false
                                    };

                                    _context.StudentNotifications.Add(studentNotification);
                                }
                            }
                        }
                    }
                }
            }

            foreach (var member in dbRequest.RegistrationRequestMembers)
            {
                if (member.Student.Status == StudentStatus.OnProcess)
                    member.Student.Status = StudentStatus.Active;

                if (member.Student.Status == StudentStatus.Inactive)
                {
                    member.Student.Status = StudentStatus.Active;

                    await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                    {
                        Uid = member.Student.FirebaseId,
                        Disabled = false
                    });
                }

                foreach (var course in dbRequest.NewCourseRequests)
                {
                    if (member.Student.ExpiryDate < course.EndDate)
                    {
                        member.Student.ExpiryDate = course.EndDate;
                    }
                }
            }

            var ec = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.CreatedByStaffId) ?? throw new NotFoundException("No EC Found.");

            var staffNotification = new StaffNotification
            {
                Staff = ec,
                RegistrationRequest = dbRequest,
                Title = "Registration Request Approval",
                Message = "The registration request has been approved. Click here for more details.",
                DateCreated = DateTime.Now,
                Type = StaffNotificationType.RegistrationRequest,
                HasRead = false
            };

            _context.StaffNotifications.Add(staffNotification);

            dbRequest.PaymentStatus = paymentStatus;
            dbRequest.RegistrationStatus = RegistrationStatus.Completed;
            dbRequest.ReviewedByStaffId = _firebaseService.GetAzureIdWithToken();
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return response;
        }

        public async Task<ServiceResponse<string>> DeclinePayment(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingOA)
                            ?? throw new NotFoundException($"PendingOA Request with ID {requestId} is not found");

            dbRequest.PaymentError = true;
            dbRequest.RegistrationStatus = RegistrationStatus.PendingEC;
            dbRequest.ReviewedByStaffId = _firebaseService.GetAzureIdWithToken();

            var ec = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Id == dbRequest.PaymentByStaffId)
                    ?? throw new NotFoundException("EC not found.");

            var oa = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Id == _firebaseService.GetAzureIdWithToken())
                    ?? throw new NotFoundException("OA not found.");

            var ecNotification = new StaffNotification
            {
                Staff = ec,
                RegistrationRequest = dbRequest,
                Title = "Payment Declined",
                Message = $"A payment for registration request ID {dbRequest.Id} has been declined by OA {oa.Nickname}. Click here for more details.",
                DateCreated = DateTime.Now,
                Type = StaffNotificationType.RegistrationRequest,
                HasRead = false
            };

            _context.StaffNotifications.Add(ecNotification);

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        public async Task<ServiceResponse<string>> CancelRequest(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s!.StudySubjects)
                                        .ThenInclude(s => s.StudySubjectMember)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(s => s!.StudySubjects)
                                        .ThenInclude(s => s.StudySubjectMember)
                            .FirstOrDefaultAsync(r => r.Id == requestId && (r.RegistrationStatus == RegistrationStatus.PendingEC || r.RegistrationStatus == RegistrationStatus.PendingEA))
                            ?? throw new NotFoundException($"PendingEC OR PendingOA Request with ID {requestId} is not found");

            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                foreach (var dbNewCourseRequest in dbRequest.NewCourseRequests)
                {
                    if (dbNewCourseRequest.StudyCourse != null)
                    {
                        dbNewCourseRequest.StudyCourse.Status = StudyCourseStatus.Cancelled;
                        foreach (var dbStudySubject in dbNewCourseRequest.StudyCourse.StudySubjects)
                        {
                            foreach (var dbStudySubjectMember in dbStudySubject.StudySubjectMember)
                            {
                                dbStudySubjectMember.Status = StudySubjectMemberStatus.Cancelled;
                            }
                        }
                    }

                }
            }
            else
            {
                var studentIdList = dbRequest.RegistrationRequestMembers.Select(s => s.StudentId).ToList();
                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    if (dbStudentAddingRequest.StudyCourse != null)
                    {
                        foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                        {
                            foreach (var dbStudySubjectMember in dbStudySubject.StudySubjectMember.Where(s => studentIdList.Contains(s.StudentId)))
                            {
                                dbStudySubjectMember.Status = StudySubjectMemberStatus.Cancelled;
                            }
                        }
                    }
                }
            }

            dbRequest.RegistrationStatus = RegistrationStatus.Cancelled;
            dbRequest.CancelledBy = _firebaseService.GetAzureIdWithToken();

            if (dbRequest.RegistrationStatus == RegistrationStatus.PendingEC)
            {
                if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
                {
                    if (dbRequest.HasSchedule)
                    {
                        var ea = await _context.Staff
                            .FirstOrDefaultAsync(s => s.Id == dbRequest.ScheduledByStaffId)
                            ?? throw new NotFoundException($"EA with ID {dbRequest.ScheduledByStaffId} not found.");

                        var eaNotificationSchedule = new StaffNotification
                        {
                            Staff = ea,
                            RegistrationRequest = dbRequest,
                            Title = "Registration Request Cancelled",
                            Message = "The registration request has been cancelled. Click here for more details.",
                            DateCreated = DateTime.Now,
                            Type = StaffNotificationType.RegistrationRequest,
                            HasRead = false
                        };

                        _context.StaffNotifications.Add(eaNotificationSchedule);
                    }
                }
            }

            if (dbRequest.RegistrationStatus == RegistrationStatus.PendingEA)
            {
                var ec = await _context.Staff
                .FirstOrDefaultAsync(s => s.Id == dbRequest.CreatedByStaffId)
                ?? throw new NotFoundException($"EC with ID {dbRequest.CreatedByStaffId} is not found.");

                var ecNotification = new StaffNotification
                {
                    Staff = ec,
                    RegistrationRequest = dbRequest,
                    Title = "Registration Request Cancelled",
                    Message = "The registration request has been cancelled. Click here for more details.",
                    DateCreated = DateTime.Now,
                    Type = StaffNotificationType.RegistrationRequest,
                    HasRead = false
                };

                _context.StaffNotifications.Add(ecNotification);

                if (dbRequest.HasSchedule)
                {
                    var ea = await _context.Staff
                            .FirstOrDefaultAsync(s => s.Id == dbRequest.ScheduledByStaffId)
                            ?? throw new NotFoundException($"EA with ID {dbRequest.ScheduledByStaffId} not found.");

                    var eaNotificationSchedule = new StaffNotification
                    {
                        Staff = ea,
                        RegistrationRequest = dbRequest,
                        Title = "Registration Request Cancelled",
                        Message = "The registration request has been cancelled. Click here for more details.",
                        DateCreated = DateTime.Now,
                        Type = StaffNotificationType.RegistrationRequest,
                        HasRead = false
                    };

                    _context.StaffNotifications.Add(eaNotificationSchedule);
                }
            }

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return response;
        }

        public async Task<ServiceResponse<string>> UpdatePayment(int requestId, UpdatePaymentRequestDto updatePayment)
        {
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .FirstOrDefaultAsync(r => r.Id == requestId
                            && r.RegistrationStatus == RegistrationStatus.Completed)
                            ?? throw new NotFoundException($"Complete Payment with ID {requestId} is not found.");

            var removeFiles = dbRequest.RegistrationRequestPaymentFiles
                                        .Where(f => updatePayment.FilesToDelete
                                        .Contains(f.FileName))
                                        .ToList();

            foreach (var file in removeFiles)
            {
                await _firebaseService.DeleteStorageFileByObjectName(file.ObjectName);
                dbRequest.RegistrationRequestPaymentFiles.Remove(file);
            }

            foreach (var newPaymentFile in updatePayment.FilesToUpload)
            {
                var objectName = await _firebaseService.UploadRegistrationRequestPaymentFile(requestId, dbRequest.DateCreated, newPaymentFile);
                if (!dbRequest.RegistrationRequestPaymentFiles.Any(f => f.ObjectName == objectName))
                {
                    dbRequest.RegistrationRequestPaymentFiles.Add(new RegistrationRequestPaymentFile()
                    {
                        FileName = newPaymentFile.FileName,
                        ObjectName = objectName,
                    });
                }
            }

            if (dbRequest.PaymentStatus != null)
                dbRequest.PaymentStatus = updatePayment.PaymentStatus;

            var oa = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Id == dbRequest.ReviewedByStaffId)
                    ?? throw new NotFoundException("OA not found.");

            var oaNotification = new StaffNotification
            {
                Staff = oa,
                RegistrationRequest = dbRequest,
                Title = $"Registration Request ID {dbRequest.Id} Payment Updated",
                Message = "A declined payment review has been updated. Click here for more details.",
                DateCreated = DateTime.Now,
                Type = StaffNotificationType.RegistrationRequest,
                HasRead = false
            };

            _context.StaffNotifications.Add(oaNotification);

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return response;
        }


        public async Task<ServiceResponse<CompletedCancellationResponseDto>> GetCompletedRequest(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == requestId)
                                                                ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            var requestDetail = new CompletedCancellationResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                PaymentType = dbRequest.PaymentType,
                PaymentStatus = dbRequest.PaymentStatus,
                PaymentError = dbRequest.PaymentError,
                ScheduleError = dbRequest.ScheduleError,
                NewCourseDetailError = dbRequest.NewCourseDetailError,
                HasSchedule = dbRequest.HasSchedule,
            };
            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId) ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

                requestDetail.StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType;

                foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        Section = dbRequestedCourse.StudyCourse?.Section,
                        CourseId = dbRequestedCourse.Course.Id,
                        Course = dbRequestedCourse.Course.course,
                        LevelId = dbRequestedCourse.LevelId,
                        Level = dbRequestedCourse.Level?.level,
                        TotalHours = dbRequestedCourse.TotalHours,
                        StartDate = dbRequestedCourse.StartDate.ToDateString(),
                        EndDate = dbRequestedCourse.EndDate.ToDateString(),
                        Method = dbRequestedCourse.Method,
                        StudyCourseType = dbRequestedCourse.StudyCourseType,
                    };
                    foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            SubjectId = dbRequestSubject.Subject.Id,
                            Subject = dbRequestSubject.Subject.subject,
                            Hour = dbRequestSubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }

                foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
                {
                    requestDetail.PreferredDays.Add(new PreferredDayResponseDto
                    {
                        Day = dbPreferredDay.Day,
                        FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                        ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                    });
                }

                requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
            }
            else
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(c => c.Subject)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId) ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        Section = dbStudentAddingRequest.StudyCourse?.Section,
                        CourseId = dbStudentAddingRequest.StudyCourse!.Course.Id,
                        Course = dbStudentAddingRequest.StudyCourse.Course.course,
                        LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                        Level = dbStudentAddingRequest.StudyCourse.Level?.level,
                        TotalHours = dbStudentAddingRequest.StudyCourse.TotalHour,
                        StartDate = dbStudentAddingRequest.StudyCourse.StartDate.ToDateString(),
                        EndDate = dbStudentAddingRequest.StudyCourse.EndDate.ToDateString(),
                        Method = dbStudentAddingRequest.StudyCourse.Method,
                        StudyCourseType = dbStudentAddingRequest.StudyCourseType,
                    };
                    foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = StudentAddingRequestMapScheduleDto(dbRequest.StudentAddingRequest);
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var objectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);

                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url,
                });
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }


            var dbStaff = await _context.Staff.ToListAsync();
            var ec = dbStaff.FirstOrDefault(s => s.Id == dbRequest.CreatedByStaffId);
            var ea = dbStaff.FirstOrDefault(s => s.Id == dbRequest.ScheduledByStaffId);
            var oa = dbStaff.FirstOrDefault(s => s.Id == dbRequest.ReviewedByStaffId);
            var cancelledBy = dbStaff.FirstOrDefault(s => s.Id == dbRequest.CancelledBy);

            if (ec != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = ec.Id,
                    Nickname = ec.Nickname,
                    FullName = ec.FullName
                };
                requestDetail.ByEC = staff;
            }
            if (ea != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = ea.Id,
                    Nickname = ea.Nickname,
                    FullName = ea.FullName
                };
                requestDetail.ByEA = staff;
            }
            if (oa != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = oa.Id,
                    Nickname = oa.Nickname,
                    FullName = oa.FullName
                };
                requestDetail.ByEA = staff;
            }
            if (cancelledBy != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = cancelledBy.Id,
                    Nickname = cancelledBy.Nickname,
                    FullName = cancelledBy.FullName
                };
                requestDetail.ByEA = staff;
            }

            var response = new ServiceResponse<CompletedCancellationResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = requestDetail,
            };
            return response;
        }

        public async Task<ServiceResponse<CompletedCancellationResponseDto>> GetCancellationRequest(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == requestId
                                                                && r.RegistrationStatus == RegistrationStatus.Cancelled)
                                                                ?? throw new NotFoundException($"Cancelled Request with ID {requestId} is not found.");

            var requestDetail = new CompletedCancellationResponseDto
            {
                RequestId = dbRequest.Id,
                Section = dbRequest.Section,
                RegistrationRequestType = dbRequest.Type,
                RegistrationStatus = dbRequest.RegistrationStatus,
                PaymentType = dbRequest.PaymentType,
                PaymentStatus = dbRequest.PaymentStatus,
                PaymentError = dbRequest.PaymentError,
                ScheduleError = dbRequest.ScheduleError,
                NewCourseDetailError = dbRequest.NewCourseDetailError,
                HasSchedule = dbRequest.HasSchedule,
            };

            if (dbRequest.Type == RegistrationRequestType.NewRequestedCourse)
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId) ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

                requestDetail.StudyCourseType = dbRequest.NewCourseRequests.ElementAt(0).StudyCourseType;

                foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        Section = dbRequestedCourse.StudyCourse?.Section,
                        CourseId = dbRequestedCourse.Course.Id,
                        Course = dbRequestedCourse.Course.course,
                        StudyCourseId = dbRequestedCourse.StudyCourse?.Id,
                        LevelId = dbRequestedCourse.LevelId,
                        Level = dbRequestedCourse.Level?.level,
                        TotalHours = dbRequestedCourse.TotalHours,
                        StartDate = dbRequestedCourse.StartDate.ToDateString(),
                        EndDate = dbRequestedCourse.EndDate.ToDateString(),
                        Method = dbRequestedCourse.Method,
                        StudyCourseType = dbRequestedCourse.StudyCourseType,
                    };
                    foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            SubjectId = dbRequestSubject.Subject.Id,
                            Subject = dbRequestSubject.Subject.subject,
                            StudySubjectId = dbRequestedCourse.StudyCourse?.StudySubjects.FirstOrDefault(s => s.SubjectId == dbRequestSubject.Subject.Id)?.Id,
                            Hour = dbRequestSubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }

                    requestDetail.Courses.Add(requestedCourse);
                }

                foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
                {
                    requestDetail.PreferredDays.Add(new PreferredDayResponseDto
                    {
                        Day = dbPreferredDay.Day,
                        FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                        ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                    });
                }

                if (requestDetail.HasSchedule != false)
                    requestDetail.Schedules = NewCourseRequestMapScheduleDto(dbRequest.NewCourseRequests);
            }
            else
            {
                dbRequest = await _context.RegistrationRequests
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(c => c.Subject)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.TeacherShifts)
                            .Include(r => r.StudentAddingRequest)
                                .ThenInclude(r => r.StudyCourse)
                                    .ThenInclude(c => c.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId) ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

                foreach (var dbStudentAddingRequest in dbRequest.StudentAddingRequest)
                {
                    var requestedCourse = new RequestedCourseResponseDto()
                    {
                        Section = dbStudentAddingRequest.StudyCourse?.Section,
                        CourseId = dbStudentAddingRequest.StudyCourse!.Course.Id,
                        Course = dbStudentAddingRequest.StudyCourse.Course.course,
                        LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                        Level = dbStudentAddingRequest.StudyCourse.Level?.level,
                        TotalHours = dbStudentAddingRequest.StudyCourse.TotalHour,
                        StartDate = dbStudentAddingRequest.StudyCourse.StartDate.ToDateString(),
                        EndDate = dbStudentAddingRequest.StudyCourse.EndDate.ToDateString(),
                        Method = dbStudentAddingRequest.StudyCourse.Method,
                        StudyCourseType = dbStudentAddingRequest.StudyCourseType,
                    };
                    foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                    {
                        var requestSubject = new RequestedSubjectResponseDto()
                        {
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        };
                        requestedCourse.subjects.Add(requestSubject);
                    }
                    requestDetail.Courses.Add(requestedCourse);
                }
                requestDetail.Schedules = StudentAddingRequestMapScheduleDto(dbRequest.StudentAddingRequest);
            }

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode!,
                    FirstName = dbMember.Student.FirstName,
                    LastName = dbMember.Student.LastName,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var objectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);

                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url,
                });
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId) ?? throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.Comment;
                requestDetail.Comments.Add(comment);
            }


            var dbStaff = await _context.Staff.ToListAsync();
            var ec = dbStaff.FirstOrDefault(s => s.Id == dbRequest.CreatedByStaffId);
            var ea = dbStaff.FirstOrDefault(s => s.Id == dbRequest.ScheduledByStaffId);
            var oa = dbStaff.FirstOrDefault(s => s.Id == dbRequest.ReviewedByStaffId);
            var cancelledBy = dbStaff.FirstOrDefault(s => s.Id == dbRequest.CancelledBy);

            if (ec != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = ec.Id,
                    Nickname = ec.Nickname,
                    FullName = ec.FullName
                };
                requestDetail.ByEC = staff;
            }
            if (ea != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = ea.Id,
                    Nickname = ea.Nickname,
                    FullName = ea.FullName
                };
                requestDetail.ByEA = staff;
            }
            if (oa != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = oa.Id,
                    Nickname = oa.Nickname,
                    FullName = oa.FullName
                };
                requestDetail.ByEA = staff;
            }
            if (cancelledBy != null)
            {
                var staff = new StaffNameOnlyResponseDto
                {
                    StaffId = cancelledBy.Id,
                    Nickname = cancelledBy.Nickname,
                    FullName = cancelledBy.FullName
                };
                requestDetail.ByEA = staff;
            }

            var response = new ServiceResponse<CompletedCancellationResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = requestDetail,
            };
            return response;
        }

        public async Task<ServiceResponse<string>> EaTakeRequest(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .FirstOrDefaultAsync(r => r.Id == requestId) ?? throw new NotFoundException("No registration request found.");

            var staffId = _firebaseService.GetAzureIdWithToken();

            dbRequest.TakenByEAId = staffId;

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        public async Task<ServiceResponse<string>> EaReleaseRequest(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                .FirstOrDefaultAsync(r => r.Id == requestId) ?? throw new NotFoundException("No registration request found.");

            dbRequest.TakenByEAId = null;

            var eas = await _context.Staff
                    .Where(s => s.Role == "ea")
                    .ToListAsync();

            foreach (var ea in eas)
            {
                var notification = new StaffNotification
                {
                    Staff = ea,
                    RegistrationRequest = dbRequest,
                    Title = "New Registration Request",
                    Message = "There is an available registration request. Click here for more details.",
                    DateCreated = DateTime.Now,
                    Type = StaffNotificationType.RegistrationRequest,
                    HasRead = false
                };

                _context.StaffNotifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        public async Task<ServiceResponse<string>> AddComment(int requestId, CommentRequestDto comment)
        {
            var response = new ServiceResponse<string>();

            var dbRequest = await _context.RegistrationRequests
                            .FirstOrDefaultAsync(r => r.Id == requestId)
                            ?? throw new NotFoundException("No Request Found.");

            var staffId = _firebaseService.GetAzureIdWithToken();

            var dbStaff = await _context.Staff
                            .FirstOrDefaultAsync(s => s.Id == staffId)
                            ?? throw new NotFoundException("No Staff Found.");

            var newComment = new RegistrationRequestComment
            {
                Staff = dbStaff,
                Comment = comment.Comment,
                DateCreated = DateTime.Now
            };

            dbRequest.RegistrationRequestComments ??= new List<RegistrationRequestComment>();
            dbRequest.RegistrationRequestComments.Add(newComment);


            switch (dbStaff.Role)
            {
                case "ec":
                    var ecNotificationEAs = await _context.Staff
                            .Where(s => s.Role == "ea")
                            .ToListAsync();

                    foreach (var ea in ecNotificationEAs)
                    {
                        var eaNotification = new StaffNotification
                        {
                            Staff = ea,
                            RegistrationRequest = dbRequest,
                            Title = "Comment",
                            Message = $"EC {dbStaff.Nickname} commented on the registration request. Click here for more details.",
                            DateCreated = DateTime.Now,
                            Type = StaffNotificationType.RegistrationRequest,
                            HasRead = false
                        };

                        _context.StaffNotifications.Add(eaNotification);
                    }

                    break;

                case "ea":
                    var eaNotificationECs = await _context.Staff
                            .Where(s => s.Role == "ec")
                            .ToListAsync();

                    foreach (var ec in eaNotificationECs)
                    {
                        var ecNotification = new StaffNotification
                        {
                            Staff = ec,
                            RegistrationRequest = dbRequest,
                            Title = "Comment",
                            Message = $"EA {dbStaff.Nickname} commented on the registration request. Click here for more details.",
                            DateCreated = DateTime.Now,
                            Type = StaffNotificationType.RegistrationRequest,
                            HasRead = false
                        };

                        _context.StaffNotifications.Add(ecNotification);
                    }

                    break;

                case "oa":
                    var oaNotificationECs = await _context.Staff
                            .Where(s => s.Role == "ec")
                            .ToListAsync();

                    foreach (var ec in oaNotificationECs)
                    {
                        var ecNotification = new StaffNotification
                        {
                            Staff = ec,
                            RegistrationRequest = dbRequest,
                            Title = "Comment",
                            Message = $"OA {dbStaff.Nickname} commented on the registration request. Click here for more details.",
                            DateCreated = DateTime.Now,
                            Type = StaffNotificationType.RegistrationRequest,
                            HasRead = false
                        };

                        _context.StaffNotifications.Add(ecNotification);
                    }

                    break;
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<RegistrationRequestCommentResponseDto>> GetCommentsByRequestId(int requestId)
        {
            var response = new ServiceResponse<RegistrationRequestCommentResponseDto>();

            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.RegistrationRequestComments)
                                .ThenInclude(r => r.Staff)
                            .FirstOrDefaultAsync(r => r.Id == requestId)
                            ?? throw new NotFoundException("No Request Found.");

            var data = new RegistrationRequestCommentResponseDto
            {
                RequestId = dbRequest.Id,
                Comments = dbRequest.RegistrationRequestComments.Select(comment => new CommentResponseDto
                {
                    StaffId = comment.Staff.Id,
                    Role = comment.Staff.Role,
                    FullName = comment.Staff.FullName,
                    CreatedAt = comment.DateCreated.ToDateTimeString(),
                    Comment = comment.Comment
                })
                .OrderByDescending(c => c.CreatedAt.ToDateTime())
                .ToList()
            };

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        // Private Service
        private List<ScheduleResponseDto> NewCourseRequestMapScheduleDto(ICollection<NewCourseRequest> requests)
        {
            var rawSchedules = new List<ScheduleResponseDto>();
            foreach (var dbRequestedCourse in requests)
            {
                if (dbRequestedCourse.StudyCourse == null)
                    throw new InternalServerException($"New Course with ID {dbRequestedCourse.Id} does not contain any StudyCourse");

                foreach (var dbStudySubject in dbRequestedCourse.StudyCourse.StudySubjects)
                {
                    foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                    {
                        var schedule = new ScheduleResponseDto()
                        {
                            Day = dbStudyClass.Schedule.Date.DayOfWeek.ToString().ToUpper(),
                            StudyCourseId = dbStudySubject.StudyCourse.Id,
                            StudyClassId = dbStudyClass.Id,
                            ClassNo = dbStudyClass.ClassNumber,
                            Room = null,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime,
                            ToTime = dbStudyClass.Schedule.ToTime,
                            CourseSubject = dbRequestedCourse.Course.course + " "
                                            + dbRequestedCourse.NewCourseSubjectRequests.First(r => r.SubjectId == dbStudySubject.SubjectId).Subject.subject
                                            + " " + (dbRequestedCourse.Level?.level ?? ""),
                            CourseId = dbRequestedCourse.Course.Id,
                            CourseName = dbRequestedCourse.Course.course,
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            SubjectName = dbStudySubject.Subject.subject,
                            Teacher = new TeacherNameResponseDto
                            {
                                TeacherId = dbStudyClass.Teacher.Id,
                                FirstName = dbStudyClass.Teacher.FirstName,
                                LastName = dbStudyClass.Teacher.LastName,
                                Nickname = dbStudyClass.Teacher.Nickname,
                                FullName = dbStudyClass.Teacher.FullName,
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
            }
            return rawSchedules.OrderBy(s => (s.Date + " " + s.FromTime).ToDateTime()).ToList();
        }

        private List<ScheduleResponseDto> StudentAddingRequestMapScheduleDto(ICollection<StudentAddingRequest> requests)
        {
            var rawSchedules = new List<ScheduleResponseDto>();
            foreach (var dbStudentAddingRequest in requests)
            {
                if (dbStudentAddingRequest.StudyCourse == null)
                    throw new InternalServerException($"Student Adding with ID {dbStudentAddingRequest.Id} does not contain any StudyCourse");

                foreach (var dbStudySubject in dbStudentAddingRequest.StudyCourse.StudySubjects)
                {
                    foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                    {
                        var schedule = new ScheduleResponseDto()
                        {
                            StudyCourseId = dbStudySubject.StudyCourse.Id,
                            Day = dbStudyClass.Schedule.Date.DayOfWeek.ToString().ToUpper(),
                            StudyClassId = dbStudyClass.Id,
                            ClassNo = dbStudyClass.ClassNumber,
                            Room = null,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime,
                            ToTime = dbStudyClass.Schedule.ToTime,
                            CourseSubject = dbStudentAddingRequest.StudyCourse.Course.course + " "
                                            + dbStudySubject.Subject.subject
                                            + " " + (dbStudentAddingRequest.StudyCourse.Level?.level ?? ""),
                            CourseId = dbStudentAddingRequest.StudyCourse.Course.Id,
                            CourseName = dbStudentAddingRequest.StudyCourse.Course.course,
                            StudySubjectId = dbStudySubject.Subject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            SubjectName = dbStudySubject.Subject.subject,
                            LevelId = dbStudentAddingRequest.StudyCourse.LevelId,
                            LevelName = dbStudentAddingRequest.StudyCourse.Level?.level,
                            Teacher = new TeacherNameResponseDto
                            {
                                TeacherId = dbStudyClass.Teacher.Id,
                                FirstName = dbStudyClass.Teacher.FirstName,
                                LastName = dbStudyClass.Teacher.LastName,
                                Nickname = dbStudyClass.Teacher.Nickname,
                                FullName = dbStudyClass.Teacher.FullName,
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
            }
            return rawSchedules.OrderBy(s => (s.Date + " " + s.FromTime).ToDateTime()).ToList();
        }
    }
}