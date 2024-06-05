using System.Globalization;
using System.Net;
using griffined_api.Dtos.ScheduleDtos;
using griffined_api.Extensions.DateTimeExtensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace griffined_api.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;
        private readonly IUnitOfWork _uow;
        private readonly IAsyncRepository<Course> _courseRepo;
        private readonly IAsyncRepository<Subject> _subjectRepo;
        private readonly IAsyncRepository<Level> _levelRepo;
        private readonly IAsyncRepository<Teacher> _teacherRepo;
        private readonly IAsyncRepository<Schedule> _scheduleRepo;
        private readonly IAsyncRepository<StudyClass> _studyClassRepo;
        private readonly IAsyncRepository<Appointment> _appointmentRepo;
        private readonly IAsyncRepository<AppointmentSlot> _appointmentSlotRepo;
        private readonly IAsyncRepository<AppointmentMember> _appointmentMemberRepo;

        public ScheduleService(DataContext context,
                               IFirebaseService firebaseService,
                               IUnitOfWork uow,
                               IAsyncRepository<Course> courseRepo,
                               IAsyncRepository<Subject> subjectRepo,
                               IAsyncRepository<Level> levelRepo,
                               IAsyncRepository<Teacher> teacherRepo,
                               IAsyncRepository<Schedule> scheduleRepo,
                               IAsyncRepository<StudyClass> studyClassRepo,
                               IAsyncRepository<Appointment> appointmentRepo,
                               IAsyncRepository<AppointmentSlot> appointmentSlotRepo,
                               IAsyncRepository<AppointmentMember> appointmentMemberRepo)
        {
            _context = context;
            _firebaseService = firebaseService;
            _uow = uow;
            _courseRepo = courseRepo;
            _subjectRepo = subjectRepo;
            _levelRepo = levelRepo;
            _teacherRepo = teacherRepo;
            _scheduleRepo = scheduleRepo;
            _studyClassRepo = studyClassRepo;
            _appointmentRepo = appointmentRepo;
            _appointmentSlotRepo = appointmentSlotRepo;
            _appointmentMemberRepo = appointmentMemberRepo;
        }

        public async Task<ServiceResponse<List<TodayMobileResponseDto>>> GetMobileTodayClass(string requestDate)
        {
            var userId = _firebaseService.GetAzureIdWithToken();
            var role = _firebaseService.GetRoleWithToken();
            List<StudyClass> dbStudyClasses = new();
            List<AppointmentSlot> dbAppointmentSlots = new();
            if (role == "teacher")
            {
                dbStudyClasses = await _context.StudyClasses
                                .Include(c => c.StudySubject)
                                    .ThenInclude(s => s.Subject)
                                .Include(c => c.StudyCourse)
                                    .ThenInclude(c => c.Course)
                                .Include(c => c.StudyCourse)
                                    .ThenInclude(c => c.Level)
                                .Include(c => c.Teacher)
                                .Include(c => c.Schedule)
                                .Where(c =>
                                c.Schedule.Date == requestDate.ToDateTime()
                                && c.Status != ClassStatus.CANCELLED
                                && c.Status != ClassStatus.DELETED
                                && c.TeacherId == userId)
                                .ToListAsync();

                dbAppointmentSlots = await _context.AppointmentSlots
                                    .Include(a => a.Appointment)
                                        .ThenInclude(a => a.AppointmentMembers)
                                    .Include(a => a.Schedule)
                                    .Where(a =>
                                    a.Schedule.Date == requestDate.ToDateTime()
                                    && a.AppointmentSlotStatus != AppointmentSlotStatus.DELETED
                                    && a.Appointment.AppointmentMembers.Any(a => a.TeacherId == userId))
                                    .ToListAsync();
            }
            else if (role == "student")
            {
                dbStudyClasses = await _context.StudyClasses
                                .Include(c => c.StudySubject)
                                    .ThenInclude(s => s.Subject)
                                .Include(c => c.StudyCourse)
                                    .ThenInclude(c => c.Course)
                                .Include(c => c.StudyCourse)
                                    .ThenInclude(c => c.Level)
                                .Include(c => c.Teacher)
                                .Include(c => c.Schedule)
                                .Where(c =>
                                c.Schedule.Date == requestDate.ToDateTime()
                                && c.Status != ClassStatus.CANCELLED
                                && c.Status != ClassStatus.DELETED
                                && c.StudySubject.StudySubjectMember.Any(m => m.StudentId == userId))
                                .ToListAsync();
            }
            else
            {
                throw new InternalServerException("Something went wrong with User.");
            }


            var data = new List<TodayMobileResponseDto>();
            foreach (var dbStudyClass in dbStudyClasses)
            {
                data.Add(new TodayMobileResponseDto
                {
                    Type = ScheduleType.Class,
                    Date = dbStudyClass.Schedule.Date.ToDateString(),
                    FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                    ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                    Class = new TodayMobileClassResponseDto
                    {
                        StudyClassId = dbStudyClass.Id,
                        StudyCourseId = dbStudyClass.StudyCourse.Id,
                        CourseId = dbStudyClass.StudyCourse.Course.Id,
                        Course = dbStudyClass.StudyCourse.Course.course,
                        StudySubjectId = dbStudyClass.StudySubject.Id,
                        SubjectId = dbStudyClass.StudySubject.Subject.Id,
                        Subject = dbStudyClass.StudySubject.Subject.subject,
                        LevelId = dbStudyClass.StudyCourse.Level?.Id,
                        Level = dbStudyClass.StudyCourse.Level?.level,
                        Section = dbStudyClass.StudyCourse.Section,
                        Room = dbStudyClass.Schedule.Room,
                        StudyCourseType = dbStudyClass.StudyCourse.StudyCourseType,
                        TeacherId = dbStudyClass.Teacher.Id,
                        TeacherFirstName = dbStudyClass.Teacher.FirstName,
                        TeacherLastName = dbStudyClass.Teacher.LastName,
                        TeacherNickname = dbStudyClass.Teacher.Nickname,
                    },
                });
            }

            foreach (var dbAppointmentSlot in dbAppointmentSlots)
            {
                data.Add(new TodayMobileResponseDto
                {
                    Type = ScheduleType.Appointment,
                    Date = dbAppointmentSlot.Schedule.Date.ToDateString(),
                    FromTime = dbAppointmentSlot.Schedule.FromTime.ToTimeSpanString(),
                    ToTime = dbAppointmentSlot.Schedule.ToTime.ToTimeSpanString(),
                    Appointment = new TodayMobileAppointmentResponseDto
                    {
                        Title = dbAppointmentSlot.Appointment.Title,
                        Description = dbAppointmentSlot.Appointment.Description,
                        AppointmentType = dbAppointmentSlot.Appointment.AppointmentType,
                    },
                });
            }

            data = data.OrderBy(d => d.FromTime.ToTimeSpan()).ToList();

            var response = new ServiceResponse<List<TodayMobileResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
            return response;
        }
        public ServiceResponse<List<DailtyCalendarDTO>> GetDailyCalendarForStaff(string requestedDate)
        {
            // GET THE LIST OF SCHEDULEIDS BASED ON THE REQUESTED DATE
            var scheduleIds = _scheduleRepo.Query()
                                           .Where(x => x.Date == requestedDate.ToDateTime())
                                           .Select(x => x.Id)
                                           .ToList();

            // GET THE STUDY CLASSES LINKED TO THE SCHEDULES
            var studyClassIds = _studyClassRepo.Query()
                                               .Where(x => scheduleIds.Contains(x.ScheduleId.Value))
                                               .Select(x => x.Id)
                                               .ToList();

            // GET THE APPOINTMENT SLOTS LINKED TO THE SCHEDULES
            var appointmentSlotIds = _appointmentSlotRepo.Query()
                                                         .Where(x => scheduleIds.Contains(x.ScheduleId.Value))
                                                         .Select(x => x.Id)
                                                         .ToList();

            var appointmentIds = _appointmentSlotRepo.Query()
                                                     .Where(x => appointmentSlotIds.Contains(x.Id))
                                                     .Select(x => x.AppointmentId)
                                                     .ToList();

            var schedules = new List<Schedule>();

            if (studyClassIds.Any())
            {
                var classSchedules = _scheduleRepo.Query()
                                                  .Where(x => studyClassIds.Contains(x.StudyClass!.Id))
                                                  .ToList();

                schedules.AddRange(classSchedules);
            }

            if (appointmentSlotIds.Any())
            {
                var appointmentSchedules = _scheduleRepo.Query()
                                                        .Where(x => appointmentSlotIds.Contains(x.AppointmentSlot!.Id))
                                                        .ToList();

                schedules.AddRange(appointmentSchedules);
            }

            var groupedSchedules = new List<TeacherScheduleGroup>();

            foreach (var schedule in schedules)
            {
                List<Teacher> teacherList = new();

                var studyClassTeacher = schedule.StudyClass?.Teacher;

                if (studyClassTeacher is not null)
                {
                    teacherList.Add(studyClassTeacher);
                }

                var appointmentTeacher = schedule.AppointmentSlot?.Appointment.AppointmentMembers.Select(m => m.Teacher)
                                                                                                 .ToList()
                                                                                                 ?? new List<Teacher>();

                teacherList.AddRange(appointmentTeacher);

                foreach (var teacher in teacherList)
                {
                    var teacherGroup = groupedSchedules.FirstOrDefault(group => group.Teacher == teacher);

                    if (teacherGroup == null)
                    {
                        teacherGroup = new TeacherScheduleGroup
                        {
                            Teacher = teacher,
                            Schedules = new List<Schedule>
                            {
                                schedule
                            }
                        };

                        groupedSchedules.Add(teacherGroup);
                    }
                    else
                    {
                        teacherGroup.Schedules.Add(schedule);
                    }
                }
            }

            var workingTeachers = new List<DailtyCalendarDTO>();

            foreach (var schedule in groupedSchedules)
            {
                var dailyCalendar = new DailtyCalendarDTO
                {
                    TeacherId = schedule.Teacher?.Id,
                    TeacherFirstName = schedule.Teacher?.FirstName,
                    TeacherLastName = schedule.Teacher?.LastName,
                    TeacherNickname = schedule.Teacher?.Nickname,
                };

                foreach (var sch in schedule.Schedules)
                {
                    foreach (var teacherShift in sch?.StudyClass?.TeacherShifts ?? new List<TeacherShift>())
                    {
                        if (sch?.StudyClass is not null && sch.StudyClass.Status != ClassStatus.CANCELLED && sch.StudyClass.Status != ClassStatus.DELETED)
                        {
                            switch (teacherShift.TeacherWorkType)
                            {
                                case TeacherWorkType.OVERTIME:
                                    dailyCalendar.OT += teacherShift.Hours;
                                    break;

                                case TeacherWorkType.SPECIAL:
                                    dailyCalendar.SP += teacherShift.Hours;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                };

                for (int i = 9; i < 20; i++)
                {
                    var firstHalf = TimeSpan.FromHours(i);
                    var secondHalf = firstHalf.Add(TimeSpan.FromMinutes(30));
                    var endHour = secondHalf.Add(TimeSpan.FromMinutes(30));
                    CalendarHalfDTO? hourSlot = null;

                    foreach (var sch in schedule.Schedules)
                    {
                        if (sch.FromTime < secondHalf
                        && firstHalf < sch.ToTime)
                        {
                            if (hourSlot == null)
                            {
                                hourSlot = new CalendarHalfDTO
                                {
                                    FirstHalf = new CalendarSlotDTO
                                    {
                                        ScheduleId = sch.Id,
                                        Time = $"{sch.FromTime.ToTimeSpanString()}-{sch.ToTime.ToTimeSpanString()}",
                                    },
                                };
                                if (sch.Type == ScheduleType.Appointment)
                                {
                                    hourSlot.FirstHalf.Name = sch.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                    if (sch.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
                                    {
                                        hourSlot.FirstHalf.Type = DailyCalendarType.HOLIDAY;
                                    }
                                    else
                                    {

                                        hourSlot.FirstHalf.Type = DailyCalendarType.OFFICE_HOURS;
                                    }
                                }
                                else
                                {
                                    hourSlot.FirstHalf.Type = sch.StudyClass!.Status switch
                                    {
                                        ClassStatus.DELETED => DailyCalendarType.CANCELLED_CLASS,
                                        ClassStatus.CANCELLED => DailyCalendarType.CANCELLED_CLASS,
                                        _ => DailyCalendarType.NORMAL_CLASS,
                                    };

                                    if (sch.StudyClass!.IsMakeup)
                                    {
                                        hourSlot.FirstHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                    }

                                    hourSlot.FirstHalf.Name = sch.StudyClass!.StudyCourse.Course.course;
                                    hourSlot.FirstHalf.Room = sch.StudyClass.Schedule.Room;
                                }
                            }
                            else
                            {
                                if (hourSlot.FirstHalf?.Type == DailyCalendarType.CANCELLED_CLASS || hourSlot.FirstHalf == null)
                                {
                                    hourSlot.FirstHalf = new CalendarSlotDTO
                                    {
                                        ScheduleId = sch.Id,
                                        Time = $"{sch.FromTime.ToTimeSpanString()}-{sch.ToTime.ToTimeSpanString()}",
                                    };

                                    if (sch.Type == ScheduleType.Appointment)
                                    {
                                        hourSlot.FirstHalf.Name = sch.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                        if (sch.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
                                        {
                                            hourSlot.FirstHalf.Type = DailyCalendarType.HOLIDAY;
                                        }
                                        else
                                        {

                                            hourSlot.FirstHalf.Type = DailyCalendarType.OFFICE_HOURS;
                                        }
                                    }
                                    else
                                    {
                                        hourSlot.FirstHalf.Type = sch.StudyClass!.Status switch
                                        {
                                            ClassStatus.DELETED => DailyCalendarType.CANCELLED_CLASS,
                                            ClassStatus.CANCELLED => DailyCalendarType.CANCELLED_CLASS,
                                            _ => DailyCalendarType.NORMAL_CLASS,
                                        };

                                        if (sch.StudyClass!.IsMakeup)
                                        {
                                            hourSlot.FirstHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                        }

                                        hourSlot.FirstHalf.Name = sch.StudyClass!.StudyCourse.Course.course;
                                        hourSlot.FirstHalf.Room = sch.StudyClass.Schedule.Room;
                                    }
                                }
                            }
                        }
                        if (sch.FromTime < endHour
                        && secondHalf < sch.ToTime)
                        {
                            if (hourSlot == null)
                            {
                                hourSlot = new CalendarHalfDTO
                                {
                                    SecondHalf = new CalendarSlotDTO
                                    {
                                        ScheduleId = sch.Id,
                                        Time = $"{sch.FromTime.ToTimeSpanString()}-{sch.ToTime.ToTimeSpanString()}",
                                    },
                                };
                                if (sch.Type == ScheduleType.Appointment)
                                {
                                    hourSlot.SecondHalf.Name = sch.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                    if (sch.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
                                    {
                                        hourSlot.SecondHalf.Type = DailyCalendarType.HOLIDAY;
                                    }
                                    else
                                    {

                                        hourSlot.SecondHalf.Type = DailyCalendarType.OFFICE_HOURS;
                                    }
                                }
                                else
                                {
                                    hourSlot.SecondHalf.Type = sch.StudyClass!.Status switch
                                    {
                                        ClassStatus.DELETED => DailyCalendarType.CANCELLED_CLASS,
                                        ClassStatus.CANCELLED => DailyCalendarType.CANCELLED_CLASS,
                                        _ => DailyCalendarType.NORMAL_CLASS,
                                    };

                                    if (sch.StudyClass!.IsMakeup)
                                    {
                                        hourSlot.SecondHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                    }

                                    hourSlot.SecondHalf.Name = sch.StudyClass!.StudyCourse.Course.course;
                                    hourSlot.SecondHalf.Room = sch.StudyClass.Schedule.Room;
                                }
                            }
                            else
                            {
                                if (hourSlot.SecondHalf?.Type == DailyCalendarType.CANCELLED_CLASS || hourSlot.SecondHalf == null)
                                {
                                    hourSlot.SecondHalf = new CalendarSlotDTO
                                    {
                                        ScheduleId = sch.Id,
                                        Time = $"{sch.FromTime.ToTimeSpanString()}-{sch.ToTime.ToTimeSpanString()}",
                                    };

                                    if (sch.Type == ScheduleType.Appointment)
                                    {
                                        hourSlot.SecondHalf.Name = sch.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                        if (sch.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
                                        {
                                            hourSlot.SecondHalf.Type = DailyCalendarType.HOLIDAY;
                                        }
                                        else
                                        {

                                            hourSlot.SecondHalf.Type = DailyCalendarType.OFFICE_HOURS;
                                        }
                                    }
                                    else
                                    {
                                        hourSlot.SecondHalf.Type = sch.StudyClass!.Status switch
                                        {
                                            ClassStatus.DELETED => DailyCalendarType.CANCELLED_CLASS,
                                            ClassStatus.CANCELLED => DailyCalendarType.CANCELLED_CLASS,
                                            _ => DailyCalendarType.NORMAL_CLASS,
                                        };

                                        if (sch.StudyClass!.IsMakeup)
                                        {
                                            hourSlot.SecondHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                        }

                                        hourSlot.SecondHalf.Name = sch.StudyClass!.StudyCourse.Course.course;
                                        hourSlot.SecondHalf.Room = sch.StudyClass.Schedule.Room;

                                    }
                                }
                            }
                        }
                    }
                    dailyCalendar.HourSlots.Add(hourSlot);
                }
                workingTeachers.Add(dailyCalendar);
            }



            // MAP TEACHER THAT IS NOT TEACHING
            var data = new List<DailtyCalendarDTO>();

            var teachers = _context.Teachers.Where(t => t.IsActive == true).ToList();

            foreach (var teacher in teachers)
            {
                var workTeacher = workingTeachers.Where(t => t.TeacherId == teacher.Id).FirstOrDefault();
                if (workTeacher != null)
                {
                    data.Add(workTeacher);
                }
                else
                {
                    data.Add(new DailtyCalendarDTO()
                    {
                        TeacherId = teacher.Id,
                        TeacherFirstName = teacher.FirstName,
                        TeacherLastName = teacher.LastName,
                        TeacherNickname = teacher.Nickname,
                    });
                }
            }

            return new ServiceResponse<List<DailtyCalendarDTO>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data.ToList(),
            };
        }

        public async Task<ServiceResponse<string>> UpdateStudyClassRoomByScheduleIds(List<UpdateRoomRequestDto> requestDto)
        {
            var dbStudyClasses = await _context.StudyClasses
                            .Include(c => c.Schedule)
                            .Where(c => requestDto.Select(r => r.ScheduleId).Contains(c.Schedule.Id))
                            .ToListAsync();

            foreach (var newRoom in requestDto)
            {
                var dbStudyClass = dbStudyClasses.FirstOrDefault(s => s.Schedule.Id == newRoom.ScheduleId)
                                    ?? throw new NotFoundException($"Schedule With ID {newRoom.ScheduleId} is not found.");
                dbStudyClass.Schedule.Room = newRoom.Room;
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        #region Generate Schedule

        public ServiceResponse<IEnumerable<AvailableAppointmentScheduleDTO>> GenerateAvailableAppointmentSchedule(CheckAvailableAppointmentScheduleDTO request)
        {
            // PARSE DATE FROM DD-MMMM-YYYY TO YYYY-MM-DD.
            var dates = request.Dates.Select(x => x.ToGregorianDateTime()).ToList();

            // PARSE DAYS INTO DAYOFWEEK ENUM.
            var daysOfWeek = request.Days?.Select(day => Enum.Parse<DayOfWeek>(day, true)).ToList() ?? new List<DayOfWeek>();

            // FETCH ALL CONFLICTING SCHEDULE IDS BASED ON THE GIVEN DATES.
            var conflictScheduleIds = _scheduleRepo.Query()
                                                   .Where(x => dates.Contains(x.Date))
                                                   .Select(x => x.Id)
                                                   .ToList();

            // FETCH ALL CONFLICTING STUDY CLASSES BASED ON THE SCHEDULE IDS AND TEACHER IDS.
            var conflictStudyClasses = _studyClassRepo.Query()
                                                      .Include(x => x.Schedule)
                                                      .Where(x => conflictScheduleIds.Contains(x.ScheduleId.Value)
                                                                  && request.TeacherIds.Contains(x.TeacherId.Value))
                                                      .ToList();

            // FETCH ALL APPOINTMENT IDS BASED ON THE TEACHER IDS.
            var appointmentIds = _appointmentMemberRepo.Query()
                                                       .Where(x => request.TeacherIds.Contains(x.TeacherId.Value))
                                                       .Select(x => x.AppointmentId)
                                                       .ToList();

            // FETCH ALL CONFLICTING APPOINTMENTS BASED ON THE APPOINTMENT IDS AND SCHEDULE IDS.
            var conflictAppointments = _appointmentSlotRepo.Query()
                                                           .Include(x => x.Schedule)
                                                           .Where(x => appointmentIds.Contains(x.AppointmentId)
                                                                       && conflictScheduleIds.Contains(x.ScheduleId.Value))
                                                           .ToList();

            // GET THE DATES OF CONFLICTING STUDY CLASSES AND APPOINTMENTS.
            var conflictDates = conflictStudyClasses.Select(x => x.Schedule.Date)
                                                    .Union(conflictAppointments.Select(x => x.Schedule.Date))
                                                    .Distinct()
                                                    .ToList();

            // FILTER OUT THE CONFLICTING DATES FROM THE REQUEST DATES
            var availableDates = dates.Except(conflictDates).ToList();

            // FILTER AVAILABLE DATES BASED ON DAYS OF WEEK
            if (daysOfWeek.Any())
            {
                availableDates = availableDates.Where(date => daysOfWeek.Contains(date.DayOfWeek)).ToList();
            }

            // GENERATE AVAILABLE SCHEDULE FOR THE NON-CONFLICTING DATES
            var availableSchedules = new List<AvailableAppointmentScheduleDTO>();
            decimal accumulatedHours = 0;

            foreach (var date in availableDates)
            {
                var hour = (decimal)(request.ToTime - request.FromTime).TotalHours;

                var availableSchedule = new AvailableAppointmentScheduleDTO
                {
                    Date = date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                    Day = date.DayOfWeek.ToString().ToUpper(),
                    FromTime = request.FromTime,
                    ToTime = request.ToTime,
                    Hour = hour,
                    ScheduleType = ScheduleType.Appointment,
                    AppointmentType = request.AppointmentType,
                    ScheduleStatus = AppointmentSlotStatus.NONE,
                    AccumulatedHour = accumulatedHours + hour // SET ACCUMULATED HOURS
                };

                accumulatedHours += hour; // UPDATE THE ACCUMULATED HOURS

                availableSchedules.Add(availableSchedule);
            }

            // CONSTRUCT ERROR MESSAGE.
            var conflictErrorMessages = new List<string>();

            foreach (var conflictingDate in conflictDates)
            {
                string conflictMessage = $"There is a scheduling conflict on {conflictingDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)}. Please choose a different date or time.";
                conflictErrorMessages.Add(conflictMessage);
            }

            string errorMessage = string.Join(" ", conflictErrorMessages);

            // THROW ERROR IF THERE ARE NO AVAILABLE SCHEDULEs.
            if (!availableSchedules.Any() || availableSchedules.Count == 0)
            {
                throw new ConflictException(errorMessage);
            }

            var response = new ServiceResponse<IEnumerable<AvailableAppointmentScheduleDTO>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = availableSchedules,
                Success = true
            };

            return response;
        }

        public ServiceResponse<IEnumerable<AvailableClassScheduleDTO>> GenerateAvailableClassSchedule(CheckAvailableClassScheduleDTO request)
        {
            // QUERY FOR TEACHER TO SEE IF REQUEST IS VALID.
            var teacher = _teacherRepo.Query()
                                      .FirstOrDefault(x => x.Id == request.TeacherId);

            if (teacher is null)
            {
                throw new NotFoundException("Teacher with given id ({request.TeacherId}) is not found.");
            }

            // PARSE DATE FROM DD-MMMM-YYYY TO YYYY-MM-DD.
            var dates = request.Dates.Select(x => x.ToGregorianDateTime()).ToList();

            // PARSE DAYS INTO DAYOFWEEK ENUM.
            var daysOfWeek = request.Days?.Select(day => Enum.Parse<DayOfWeek>(day, true)).ToList() ?? new List<DayOfWeek>();

            // FETCH ALL CONFLICTING SCHEDULE IDS BASED ON THE GIVEN DATES.
            var conflictScheduleIds = _scheduleRepo.Query()
                                                   .Where(x => dates.Contains(x.Date))
                                                   .Select(x => x.Id)
                                                   .ToList();

            // FETCH ALL CONFLICTING STUDY CLASSES BASED ON THE SCHEDULE IDS AND TEACHER IDS.
            var conflictStudyClasses = _studyClassRepo.Query()
                                                      .Include(x => x.Schedule)
                                                      .Where(x => conflictScheduleIds.Contains(x.ScheduleId.Value)
                                                                  && x.TeacherId == request.TeacherId)
                                                      .ToList();

            // FETCH ALL CONFLICTING APPOINTMENT IDS BASED ON THE TEACHER IDS.
            var conflictAppointmentIds = _appointmentMemberRepo.Query()
                                                               .Where(x => x.TeacherId == request.TeacherId)
                                                               .Select(x => x.AppointmentId)
                                                               .ToList();

            // FETCH ALL CONFLICTING APPOINTMENTS BASED ON THE APPOINTMENT IDS AND SCHEDULE IDS.
            var conflictAppointments = _appointmentSlotRepo.Query()
                                                           .Include(x => x.Schedule)
                                                           .Where(x => conflictAppointmentIds.Contains(x.AppointmentId)
                                                                       && conflictScheduleIds.Contains(x.ScheduleId.Value))
                                                           .ToList();

            // QUERY FOR COURSE, SUBJECT, AND LEVEL.
            var course = _courseRepo.Query()
                                    .FirstOrDefault(x => x.Id == request.CourseId);

            if (course is null)
            {
                throw new NotFoundException($"Course with given id ({request.CourseId}) is not found.");
            }

            var subject = _subjectRepo.Query()
                                      .FirstOrDefault(x => x.Id == request.SubjectId
                                                        && x.CourseId == request.CourseId);

            if (subject is null)
            {
                throw new NotFoundException($"Subject with given id ({request.SubjectId}) with course id ({request.CourseId}) is not found.");
            }

            Level? level = null;

            if (request.LevelId is not null)
            {

                level = _levelRepo.Query()
                                  .FirstOrDefault(x => x.Id == request.LevelId.Value
                                                    && x.CourseId == request.CourseId);

                if (level is null)
                {
                    throw new NotFoundException($"Level with given id ({request.LevelId}) with course id ({request.CourseId}) is not found.");
                }
            }

            // GET THE DATES OF CONFLICTING STUDY CLASSES AND APPOINTMENTS.
            var conflictDates = conflictStudyClasses.Select(x => x.Schedule.Date)
                                                    .Union(conflictAppointments.Select(x => x.Schedule.Date))
                                                    .Distinct()
                                                    .ToList();

            // FILTER OUT THE CONFLICTING DATES FROM THE REQUEST DATES
            var availableDates = dates.Except(conflictDates).ToList();

            // FILTER AVAILABLE DATES BASED ON DAYS OF WEEK
            if (daysOfWeek.Any())
            {
                availableDates = availableDates.Where(date => daysOfWeek.Contains(date.DayOfWeek)).ToList();
            }

            var availableSchedules = new List<AvailableClassScheduleDTO>();
            decimal accumulatedHours = 0;

            foreach (var date in availableDates)
            {
                var hour = (decimal)(request.ToTime - request.FromTime).TotalHours;

                var availableSchedule = new AvailableClassScheduleDTO
                {
                    CourseId = request.CourseId,
                    CourseName = course.course,
                    SubjectId = request.SubjectId,
                    SubjectName = subject.subject,
                    LevelId = request.LevelId is null ? 0
                                                      : request.LevelId.Value,
                    LevelName = level is null ? null
                                              : level.level,
                    Date = date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                    Day = date.DayOfWeek.ToString().ToUpper(),
                    FromTime = request.FromTime,
                    ToTime = request.ToTime,
                    Hour = hour,
                    ScheduleType = ScheduleType.Class,
                    ScheduleStatus = ClassStatus.NONE,
                    AccumulatedHour = accumulatedHours + hour // SET ACCUMULATED HOURS
                };

                accumulatedHours += hour; // UPDATE THE ACCUMULATED HOURS

                availableSchedules.Add(availableSchedule);
            }

            // CONSTRUCT ERROR MESSAGE.
            var conflictErrorMessages = new List<string>();

            foreach (var conflictingDate in conflictDates)
            {
                string conflictMessage = $"There is a scheduling conflict on {conflictingDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)}. Please choose a different date or time.";
                conflictErrorMessages.Add(conflictMessage);
            }

            string errorMessage = string.Join(" ", conflictErrorMessages);

            // THROW ERROR IF THERE ARE NO AVAILABLE SCHEDULEs.
            if (!availableSchedules.Any() || availableSchedules.Count == 0)
            {
                throw new ConflictException(errorMessage);
            }

            var response = new ServiceResponse<IEnumerable<AvailableClassScheduleDTO>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = availableSchedules,
                Success = true
            };

            return response;
        }

        #endregion
    }
}
