using System.Globalization;
using System.Net;
using griffined_api.Dtos.ScheduleDtos;
using griffined_api.Extensions.DateTimeExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Quartz.Impl.Calendar;

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
        private readonly IAsyncRepository<StudyCourse> _studyCourseRepo;
        private readonly IAsyncRepository<StudySubject> _studySubjectRepo;
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
                               IAsyncRepository<StudyCourse> studyCourseRepo,
                               IAsyncRepository<StudySubject> studySubjectRepo,
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
            _studyCourseRepo = studyCourseRepo;
            _studySubjectRepo = studySubjectRepo;
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
            var studyClasses = _studyClassRepo.Query()
                                              .Where(x => x.ScheduleId != null && scheduleIds.Contains(x.ScheduleId.Value))
                                              .ToList();

            var studyClassIds = studyClasses.Select(x => x.Id);

            // GET THE APPOINTMENT SLOTS LINKED TO THE SCHEDULES
            var appointmentSlotIds = _appointmentSlotRepo.Query()
                                                         .Where(x => x.ScheduleId != null && scheduleIds.Contains(x.ScheduleId.Value))
                                                         .Select(x => x.Id)
                                                         .ToList();

            var appointments = _appointmentRepo.Query()
                                               .Where(x => appointmentSlotIds.Contains(x.Id))
                                               .ToList();

            var appointmentIds = appointments.Select(x => x.Id).ToList();

            var appointmentMember = _appointmentMemberRepo.Query()
                                                          .Include(x => x.Teacher)
                                                          .Where(x => appointmentIds.Contains(x.Id))
                                                          .ToList();


            var teachers = _teacherRepo.Query().ToList();


            // CREATE TEACHER DICT
            var teacherDict = new Dictionary<int, Teacher>();

            foreach (var teacher in teachers)
            {
                teacherDict[teacher.Id] = teacher;
            }

            var schedules = new List<Schedule>();

            if (studyClassIds.Any())
            {
                var classSchedules = _scheduleRepo.Query()
                                                  .Include(x => x.StudyClass)
                                                  .Where(x => x.StudyClass != null && studyClassIds.Contains(x.StudyClass.Id))
                                                  .ToList();

                schedules.AddRange(classSchedules);
            }

            if (appointmentSlotIds.Any())
            {
                var appointmentSchedules = _scheduleRepo.Query()
                                                        .Include(x => x.AppointmentSlot)
                                                        .Where(x => x.AppointmentSlot != null && appointmentSlotIds.Contains(x.AppointmentSlot.Id))
                                                        .ToList();

                foreach (var classSchedule in schedules)
                {
                    if (classSchedule.AppointmentSlot != null)
                    {
                        var appoint = appointments.FirstOrDefault(x => x.Id == classSchedule.AppointmentSlot?.AppointmentId);

                        if (appoint != null)
                        {
                            // MAP APPOINTMENT INTO SCHEDULE DATA
                            classSchedule.AppointmentSlot.Appointment = appoint;
                        }
                    }

                }

                schedules.AddRange(appointmentSchedules);
            }

            var groupedSchedules = new List<TeacherScheduleGroup>();

            foreach (var schedule in schedules)
            {
                List<Teacher> teacherList = new();

                var studyClassTeacherIds = schedule.StudyClass?.TeacherId;

                if (studyClassTeacherIds is int teacherId)
                {
                    teacherList.Add(teacherDict[teacherId]);
                }

                var appointmentTeacher = appointmentMember.Where(x => appointmentIds.Contains((int)x.AppointmentId))
                                                          .Select(a => a.Teacher)
                                                          .ToList();

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

            var studySubjectIds = studyClasses.Select(x => x.StudySubjectId);

            var studyCourseIds = studyClasses.Select(x => x.StudyCourseId);

            var studyCourses = _studyCourseRepo.Query()
                                               .Include(x => x.Course)
                                               .Include(x => x.Level)
                                               .Where(x => studyCourseIds.Contains(x.Id))
                                               .ToList();

            var studySubjects = _studySubjectRepo.Query()
                                               .Include(x => x.Subject)
                                               .Where(x => studySubjectIds.Contains(x.Id))
                                               .ToList();

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


                // Time in timetable - 9:00 to 20:00
                for (int i = 9; i < 20; i++)
                {
                    var firstHalf = TimeSpan.FromHours(i);
                    var secondHalf = firstHalf.Add(TimeSpan.FromMinutes(30));
                    var endHour = secondHalf.Add(TimeSpan.FromMinutes(30));
                    CalendarHalfDTO? hourSlot = null;

                    // IF EMPTY HOUR SLOT THEN ADD WHAT EVER IT IS
                    // IF IT IS NOT EMPTY THEN COMPARE WEIGHT
                    foreach (var sch in schedule.Schedules)
                    {
                        // MAP FIRST HALF OF HOUR
                        if (sch.FromTime < secondHalf
                        && firstHalf < sch.ToTime)
                        {
                            if (hourSlot == null)
                            {
                                hourSlot = new CalendarHalfDTO
                                {
                                    FirstHalf = MapHalf(sch, studyCourses, studySubjects)
                                };
                            }
                            else
                            {
                                hourSlot.FirstHalf ??= new CalendarSlotDTO();
                                if (hourSlot.FirstHalf.Type < sch.CalendarType || hourSlot.FirstHalf.Type == null)
                                {
                                    hourSlot.FirstHalf = MapHalf(sch, studyCourses, studySubjects);
                                }
                            }
                        }

                        // MAP SECOND HALF OF HOUR
                        if (sch.FromTime < endHour
                        && secondHalf < sch.ToTime)
                        {
                            // IF EMPTY HOUR SLOT
                            if (hourSlot == null)
                            {
                                hourSlot = new CalendarHalfDTO
                                {
                                    SecondHalf = MapHalf(sch, studyCourses, studySubjects)
                                };
                            }
                            else
                            {
                                hourSlot.SecondHalf ??= new CalendarSlotDTO();
                                if (hourSlot.SecondHalf.Type < sch.CalendarType || hourSlot.SecondHalf.Type == null)
                                {
                                    hourSlot.SecondHalf = MapHalf(sch, studyCourses, studySubjects);
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

            foreach (var teacher in teachers)
            {
                if (!teacher.IsActive)
                {
                    continue;
                }
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

        private static CalendarSlotDTO? MapHalf(
            Schedule sch,
            List<StudyCourse> studyCourses,
            List<StudySubject> studySubjects
        )
        {
            CalendarSlotDTO? hourSlot = null;
            switch (sch.CalendarType)
            {
                case DailyCalendarType.HOLIDAY:
                    hourSlot = new CalendarSlotDTO
                    {
                        ScheduleId = sch.Id,
                        Type = sch.CalendarType,
                        Room = sch.Room,
                        Time = $"{sch.FromTime.ToTimeSpanString()}-{sch.ToTime.ToTimeSpanString()}",
                        Name = sch.AppointmentSlot?.Appointment.AppointmentType.ToString() ?? "HOLIDAY",
                    };

                    break;

                case DailyCalendarType.EVENT:
                    hourSlot = new CalendarSlotDTO
                    {
                        ScheduleId = sch.Id,
                        Type = sch.CalendarType,
                        Room = sch.Room,
                        Time = $"{sch.FromTime.ToTimeSpanString()}-{sch.ToTime.ToTimeSpanString()}",
                        Name = sch.AppointmentSlot?.Appointment.AppointmentType.ToString() ?? "EVENT",
                    };
                    break;

                case DailyCalendarType.MAKEUP_CLASS or
                     DailyCalendarType.NORMAL_CLASS or
                     DailyCalendarType.CANCELLED_CLASS or
                     DailyCalendarType.SUBSTITUTE:

                    hourSlot = new CalendarSlotDTO
                    {
                        ScheduleId = sch.Id,
                        Type = sch.CalendarType,
                        Room = sch.Room,
                        Time = $"{sch.FromTime.ToTimeSpanString()}-{sch.ToTime.ToTimeSpanString()}",
                    };

                    // Concat course name
                    var slotName = "";
                    if (sch.StudyClass != null)
                    {
                        var name = new List<string>();
                        var studyCourse = studyCourses.FirstOrDefault(x => x.Id == sch.StudyClass.StudyCourseId);
                        if (studyCourse != null)
                        {
                            name.Add(studyCourse.Course.course);
                        }

                        var studySubject = studySubjects.FirstOrDefault(x => x.Id == sch.StudyClass.StudySubjectId);
                        if (studySubject != null)
                        {
                            name.Add(studySubject.Subject.subject);
                        }

                        if (studyCourse != null && studyCourse.Level != null)
                        {
                            name.Add(studyCourse.Level.level);
                        }
                        slotName = string.Join(" ", name);
                    }
                    hourSlot.Name = slotName;
                    break;

                default:
                    break;
            }
            return hourSlot;
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

            // GET ALL HOLIDAYS
            var holidayDates = _scheduleRepo.Query()
                                            .Where(x => x.CalendarType == DailyCalendarType.HOLIDAY
                                                        && conflictDates.Contains(x.Date))
                                            .Select(x => x.Date)
                                            .ToList();

            // PREPARE A DICTIONARY TO HOLD CONFLICT DATES AND ASSOCIATED TEACHERS.
            var conflictDetails = new Dictionary<DateTime, List<string>>();

            // ADD CONFLICTING STUDY CLASSES' TEACHER DETAILS.
            foreach (var studyClass in conflictStudyClasses)
            {
                var date = studyClass.Schedule.Date;

                var teacherNickname = _teacherRepo.Query()
                                                  .Where(x => x.Id == studyClass.TeacherId)
                                                  .Select(x => x.Nickname)
                                                  .FirstOrDefault();

                if (!conflictDetails.ContainsKey(date))
                {
                    conflictDetails[date] = new List<string>();
                }

                if (!string.IsNullOrEmpty(teacherNickname))
                {
                    conflictDetails[date].Add($"T. {teacherNickname}");
                }
            }

            // ADD CONFLICTING APPOINTMENTS' TEACHER DETAILS.
            foreach (var appointment in conflictAppointments)
            {
                var date = appointment.Schedule.Date;

                var teacherIds = _appointmentMemberRepo.Query()
                                                       .Where(x => x.AppointmentId == appointment.AppointmentId)
                                                       .Select(x => x.TeacherId)
                                                       .ToList();

                foreach (var teacherId in teacherIds)
                {
                    var teacherNickname = _teacherRepo.Query()
                                                      .Where(x => x.Id == teacherId)
                                                      .Select(x => x.Nickname)
                                                      .FirstOrDefault();

                    if (!conflictDetails.ContainsKey(date))
                    {
                        conflictDetails[date] = new List<string>();
                    }

                    if (!string.IsNullOrEmpty(teacherNickname))
                    {
                        conflictDetails[date].Add($"T. {teacherNickname}");
                    }
                }
            }

            // CONSTRUCT THE FINAL FORMATTED DATES FOR THE ERROR MESSAGE.
            var formattedDates = conflictDates.Select(date =>
            {
                if (holidayDates.Contains(date))
                {
                    return $"{date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)} (holiday)";
                }

                var teacherDetails = conflictDetails.ContainsKey(date) ? string.Join(", ", conflictDetails[date]) : "";

                return !string.IsNullOrEmpty(teacherDetails)
                    ? $"{date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)} ({teacherDetails})"
                    : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
            })
            .ToList();


            // JOIN ALL THE DATES USING COMMAS.
            string formattedDateString = string.Join(", ", formattedDates);

            // CONSTRUCT THE ERROR MESSAGE.
            string errorMessage = $"There is a scheduling conflict on {formattedDateString}. Please choose a different date or time.";

            if (conflictDates.Any())
            {

                throw new ConflictException(errorMessage);
            }

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

                // CHECK IF THIS SCHEDULE ALREADY EXISTS IN THE CURRENT SCHEDULES.
                bool alreadyExists = request.CurrentSchedules?.Any(x => x.Date == availableSchedule.Date &&
                                                                        x.Day == availableSchedule.Day &&
                                                                        x.FromTime == availableSchedule.FromTime &&
                                                                        x.ToTime == availableSchedule.ToTime &&
                                                                        x.AppointmentType == availableSchedule.AppointmentType)
                                                                        ?? false;

                if (!alreadyExists)
                {
                    availableSchedules.Add(availableSchedule);
                }
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

            // CHECK FOR EXISTING SCHEDULES TO AVOID DUPLICATES
            var currentSchedules = request.CurrentSchedules ?? new List<AvailableClassScheduleDTO>();

            // FILTER OUT REQUESTED DATES THAT ALREADY EXIST IN THE CURRENT SCHEDULES.
            var nonConflictingDates = dates.Except(conflictDates).ToList();

            var availableDates = nonConflictingDates.Where(x =>
                !currentSchedules.Any(y =>
                    y.Date == x.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) &&
                    y.FromTime == request.FromTime &&
                    y.ToTime == request.ToTime
                )).ToList();

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

            // GET ALL HOLIDAYS
            var holidayDates = _scheduleRepo.Query()
                                            .Where(x => x.CalendarType == DailyCalendarType.HOLIDAY
                                                        && conflictDates.Contains(x.Date))
                                            .Select(x => x.Date)
                                            .ToList();

            // PREPARE A DICTIONARY TO HOLD CONFLICT DATES AND ASSOCIATED TEACHERS.
            var conflictDetails = new Dictionary<DateTime, List<string>>();

            // ADD CONFLICTING STUDY CLASSES' TEACHER DETAILS.
            foreach (var studyClass in conflictStudyClasses)
            {
                var date = studyClass.Schedule.Date;

                var teacherNickname = _teacherRepo.Query()
                                                  .Where(x => x.Id == studyClass.TeacherId)
                                                  .Select(x => x.Nickname)
                                                  .FirstOrDefault();

                if (!conflictDetails.ContainsKey(date))
                {
                    conflictDetails[date] = new List<string>();
                }

                if (!string.IsNullOrEmpty(teacherNickname))
                {
                    conflictDetails[date].Add($"T. {teacherNickname}");
                }
            }

            // ADD CONFLICTING APPOINTMENTS' TEACHER DETAILS.
            foreach (var appointment in conflictAppointments)
            {
                var date = appointment.Schedule.Date;

                var teacherIds = _appointmentMemberRepo.Query()
                                                       .Where(x => x.AppointmentId == appointment.AppointmentId)
                                                       .Select(x => x.TeacherId)
                                                       .ToList();

                foreach (var teacherId in teacherIds)
                {
                    var teacherNickname = _teacherRepo.Query()
                                                      .Where(x => x.Id == teacherId)
                                                      .Select(x => x.Nickname)
                                                      .FirstOrDefault();

                    if (!conflictDetails.ContainsKey(date))
                    {
                        conflictDetails[date] = new List<string>();
                    }

                    if (!string.IsNullOrEmpty(teacherNickname))
                    {
                        conflictDetails[date].Add($"T. {teacherNickname}");
                    }
                }
            }

            // CONSTRUCT THE FINAL FORMATTED DATES FOR THE ERROR MESSAGE.
            var formattedDates = conflictDates.Select(date =>
            {
                if (holidayDates.Contains(date))
                {
                    return $"{date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)} (holiday)";
                }

                var teacherDetails = conflictDetails.ContainsKey(date) ? string.Join(", ", conflictDetails[date]) : "";

                return !string.IsNullOrEmpty(teacherDetails)
                    ? $"{date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)} ({teacherDetails})"
                    : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
            })
            .ToList();


            // JOIN ALL THE DATES USING COMMAS.
            string formattedDateString = string.Join(", ", formattedDates);

            // CONSTRUCT THE ERROR MESSAGE.
            string errorMessage = $"There is a scheduling conflict on {formattedDateString}. Please choose a different date or time.";

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
