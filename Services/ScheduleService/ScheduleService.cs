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
        private readonly ITeacherService _teacherService;
        private readonly IUnitOfWork _uow;
        private readonly IAsyncRepository<Course> _courseRepo;
        private readonly IAsyncRepository<Subject> _subjectRepo;
        private readonly IAsyncRepository<Level> _levelRepo;
        private readonly IAsyncRepository<Student> _studentRepo;
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
                               ITeacherService teacherService,
                               IUnitOfWork uow,
                               IAsyncRepository<Course> courseRepo,
                               IAsyncRepository<Subject> subjectRepo,
                               IAsyncRepository<Level> levelRepo,
                               IAsyncRepository<Student> studentRepo,
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
            _teacherService = teacherService;
            _uow = uow;
            _courseRepo = courseRepo;
            _subjectRepo = subjectRepo;
            _levelRepo = levelRepo;
            _studentRepo = studentRepo;
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
            var appointmentSlots = _appointmentSlotRepo.Query()
                                                         .Where(x => x.ScheduleId != null && scheduleIds.Contains(x.ScheduleId.Value))
                                                         .ToList();
            var appointmentIds = appointmentSlots.Select(x => x.AppointmentId)
                                                    .ToList();

            var appointmentSlotIds = appointmentSlots.Select(x => x.Id)
                                                    .ToList();

            var appointments = _appointmentRepo.Query()
                                               .Where(x => appointmentIds.Contains(x.Id))
                                               .ToList();


            var appointmentMember = _appointmentMemberRepo.Query()
                                                          .Include(x => x.Teacher)
                                                          .Where(x => appointmentIds.Contains(x.AppointmentId))
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

                foreach (var classSchedule in appointmentSchedules)
                {
                    if (classSchedule.AppointmentSlot != null)
                    {
                        var appoint = appointments.FirstOrDefault(x => x.Id == classSchedule.AppointmentSlot?.AppointmentId);

                        if (appoint != null)
                        {
                            // MAP APPOINTMENT INTO SCHEDULE DATA
                            classSchedule.AppointmentSlot.Appointment = appoint;

                            if (appoint.AppointmentType == AppointmentType.HOLIDAY)
                            {
                                classSchedule.FromTime = new TimeSpan(8, 0, 0);  // 8 hours, 0 minutes, 0 seconds
                                classSchedule.ToTime = new TimeSpan(20, 0, 0);  // 20 hours, 0 minutes, 0 seconds
                            }
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
                        if (sch?.StudyClass is not null && sch.StudyClass.Status != ClassStatus.CANCELLED
                                                        && sch.StudyClass.Status != ClassStatus.DELETED)
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
                }

                // Time in timetable - 8:00 to 20:00
                for (int i = 8; i < 20; i++)
                {
                    var firstHalf = TimeSpan.FromHours(i);
                    var secondHalf = firstHalf.Add(TimeSpan.FromMinutes(30));
                    var endHour = secondHalf.Add(TimeSpan.FromMinutes(30));
                    CalendarHalfDTO hourSlot = new CalendarHalfDTO();

                    // IF EMPTY HOUR SLOT THEN ADD WHATEVER IT IS
                    // IF IT IS NOT EMPTY THEN COMPARE WEIGHT
                    foreach (var sch in schedule.Schedules)
                    {
                        // IF SCHEDULE IS DELETED
                        if (sch.CalendarType == DailyCalendarType.DELETED)
                        {
                            continue;
                        }

                        // MAP FIRST HALF OF HOUR
                        if (sch.FromTime < secondHalf
                            && firstHalf < sch.ToTime)
                        {
                            if (hourSlot.FirstHalf == null || hourSlot.FirstHalf.Type < sch.CalendarType)
                            {
                                hourSlot.FirstHalf = MapHalf(sch, studyCourses, studySubjects);
                            }
                        }

                        // MAP SECOND HALF OF HOUR
                        if (sch.FromTime < endHour
                            && secondHalf < sch.ToTime)
                        {
                            if (hourSlot.SecondHalf == null || hourSlot.SecondHalf.Type < sch.CalendarType)
                            {
                                hourSlot.SecondHalf = MapHalf(sch, studyCourses, studySubjects);
                            }
                        }
                    }

                    // If hourSlot is still null, set it to OFFICE_HOURS
                    // if (hourSlot.FirstHalf == null)
                    // {
                    //     hourSlot.FirstHalf = new CalendarSlotDTO
                    //     {
                    //         Type = DailyCalendarType.OFFICE_HOURS,
                    //         Time = $"{firstHalf:hh\\:mm}-{secondHalf:hh\\:mm}",
                    //         Name = "Office Hours"
                    //     };
                    // }

                    // if (hourSlot.SecondHalf == null)
                    // {
                    //     hourSlot.SecondHalf = new CalendarSlotDTO
                    //     {
                    //         Type = DailyCalendarType.OFFICE_HOURS,
                    //         Time = $"{secondHalf:hh\\:mm}-{endHour:hh\\:mm}",
                    //         Name = "Office Hours"
                    //     };
                    // }

                    if ((hourSlot.FirstHalf == null) && (hourSlot.SecondHalf == null))
                    {
                        dailyCalendar.HourSlots.Add(null);
                    }
                    else
                    {
                        dailyCalendar.HourSlots.Add(hourSlot);
                    }
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

                var workTeacher = workingTeachers.FirstOrDefault(t => t.TeacherId == teacher.Id);

                if (workTeacher != null)
                {
                    data.Add(workTeacher);
                }
                else
                {
                    // Add OFFICE_HOURS for teachers without schedules
                    var dailyCalendar = new DailtyCalendarDTO
                    {
                        TeacherId = teacher.Id,
                        TeacherFirstName = teacher.FirstName,
                        TeacherLastName = teacher.LastName,
                        TeacherNickname = teacher.Nickname,
                        // Time in timetable - 8:00 to 20:00
                        HourSlots = Enumerable.Repeat<CalendarHalfDTO?>(null, 12).ToList()
                    };
                    // for (int i = 8; i < 20; i++)
                    // {
                    //     var firstHalf = TimeSpan.FromHours(i);
                    //     var secondHalf = firstHalf.Add(TimeSpan.FromMinutes(30));
                    //     var endHour = secondHalf.Add(TimeSpan.FromMinutes(30));
                    //     // CalendarHalfDTO hourSlot = new()
                    //     // {
                    //     //     FirstHalf = null,
                    //     //     SecondHalf = null,
                    //     // };
                    //     // CalendarHalfDTO hourSlot = new CalendarHalfDTO
                    //     // {
                    //     //     FirstHalf = new CalendarSlotDTO
                    //     //     {
                    //     //         Type = DailyCalendarType.OFFICE_HOURS,
                    //     //         Time = $"{firstHalf:hh\\:mm}-{secondHalf:hh\\:mm}",
                    //     //         Name = "Office Hours"
                    //     //     },
                    //     //     SecondHalf = new CalendarSlotDTO
                    //     //     {
                    //     //         Type = DailyCalendarType.OFFICE_HOURS,
                    //     //         Time = $"{secondHalf:hh\\:mm}-{endHour:hh\\:mm}",
                    //     //         Name = "Office Hours"
                    //     //     }
                    //     // };

                    //     dailyCalendar.HourSlots.Add(null);
                    // }

                    data.Add(dailyCalendar);
                }
            }

            // Define the time slots
            var timeSlots = new Dictionary<int, (string firstHalf, string secondHalf)>
            {
                { 0, ("8:00-8:30", "8:30-9:00") },
                { 1, ("9:00-9:30", "9:30-10:00") },
                { 2, ("10:00-10:30", "10:30-11:00") },
                { 3, ("11:00-11:30", "11:30-12:00") },
                { 4, ("12:00-12:30", "12:30-13:00") },
                { 5, ("13:00-13:30", "13:30-14:00") },
                { 6, ("14:00-14:30", "14:30-15:00") },
                { 7, ("15:00-15:30", "15:30-16:00") },
                { 8, ("16:00-16:30", "16:30-17:00") },
                { 9, ("17:00-17:30", "17:30-18:00") },
                { 10, ("18:00-18:30", "18:30-19:00") },
                { 11, ("19:00-19:30", "19:30-20:00") }
            };


            // MAP OFFICE HOURS
            var idx = -1;
            foreach (var teacher in data)
            {

                idx++;

                var officeHourSlot = new List<int> { 3, 6, 7, 8, 9 }; // Add absolute office hour 
                if (teacher.HourSlots[4] == null) // IF 12:00 - 13:00 there is no class then add office to 13:00 - 14:00
                {
                    officeHourSlot.Add(5);
                }

                if (teacher.HourSlots[11] != null) //IF 19:00 - 20:00 is not null
                {
                    officeHourSlot.Add(10);
                    AssignSlots(teacher, data, idx, officeHourSlot);
                    continue;
                }

                if (teacher.HourSlots[10] != null) //IF 18:00 - 19:00 is not null
                {
                    officeHourSlot.Add(2);
                    AssignSlots(teacher, data, idx, officeHourSlot);
                    continue;
                }

                if (teacher.HourSlots[1] != null) //IF 9:00 - 10:00 is not null
                {
                    AssignSlots(teacher, data, idx, officeHourSlot);
                    continue;
                }

                // DEFAULT CASE 
                officeHourSlot.Add(2); // ADD 10:00 - 11:00 Slot

                officeHourSlot.Add(10); // ADD 18:00 - 19:00 Slot
                AssignSlots(teacher, data, idx, officeHourSlot);
            }

            void AssignSlots(dynamic teacher, dynamic data, int idx, List<int> slots)
            {
                slots.Sort();
                for (int i = 0; i < slots.Count; i++)
                {
                    var slot = slots[i];
                    if (teacher.HourSlots[slot] == null)
                    {
                        var startSlot = slot;
                        var endSlot = slot;

                        // Check for consecutive office hour slots
                        while (i + 1 < slots.Count && slots[i + 1] == endSlot + 1 && teacher.HourSlots[endSlot + 1] == null)
                        {
                            endSlot++;
                            i++;
                        }

                        var startTime = timeSlots[startSlot].firstHalf.Split('-')[0].Trim();
                        var endTime = timeSlots[endSlot].secondHalf.Split('-')[1].Trim();
                        var mergedTime = $"{startTime}-{endTime}";

                        var officeHour = new CalendarHalfDTO
                        {
                            FirstHalf = new CalendarSlotDTO
                            {
                                Type = DailyCalendarType.OFFICE_HOURS,
                                Name = "Office Hours",
                                Time = mergedTime
                            },
                            SecondHalf = new CalendarSlotDTO
                            {
                                Type = DailyCalendarType.OFFICE_HOURS,
                                Name = "Office Hours",
                                Time = mergedTime
                            }
                        };

                        // Assign office hours to all slots in the range
                        for (int j = startSlot; j <= endSlot; j++)
                        {
                            data[idx].HourSlots[j] = officeHour;
                        }
                    }
                }
            }


            return new ServiceResponse<List<DailtyCalendarDTO>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data.ToList(),
            };
        }

        private static CalendarSlotDTO? MapHalf(Schedule sch,
                                                List<StudyCourse> studyCourses,
                                                List<StudySubject> studySubjects)
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
            var dbStudyClasses = await _context.StudyClasses.Include(c => c.Schedule)
                                                            .Where(c => requestDto.Select(r => r.ScheduleId)
                                                                                  .Contains(c.Schedule.Id))
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

        public ServiceResponse<AvailableAppointmentDTO> GenerateAvailableAppointmentSchedule(CheckAvailableAppointmentScheduleDTO request)
        {
            // PARSE DATE FROM DD-MMMM-YYYY TO YYYY-MM-DD.
            var dates = request.Dates.Select(x => x.ToGregorianDateTime()).ToList();

            // PARSE DAYS INTO DAYOFWEEK ENUM.
            var daysOfWeek = request.Days?.Select(day => Enum.Parse<DayOfWeek>(day, true)).ToList() ?? new List<DayOfWeek>();

            // FILTER AVAILABLE DATES BASED ON DAYS OF WEEK
            if (daysOfWeek.Any())
            {
                dates = dates.Where(date => daysOfWeek.Contains(date.DayOfWeek)).ToList();
            }

            List<DateTime> conflictDates = new();

            // PREPARE AN EMPTY LIST FOR CONFLICTED SCHEDULES.
            var conflictedSchedules = new List<ConflictScheduleDTO>();

            if (request.AppointmentType != AppointmentType.HOLIDAY)
            {
                // FETCH ALL CONFLICTING SCHEDULE IDS BASED ON THE GIVEN DATES.
                var conflictScheduleIds = _scheduleRepo.Query()
                                                       .Where(x => dates.Contains(x.Date)
                                                                && x.FromTime < request.ToTime && x.ToTime > request.FromTime)
                                                       .Select(x => x.Id)
                                                       .ToList();

                // FETCH ALL CONFLICTING STUDY CLASSES BASED ON THE SCHEDULE IDS AND TEACHER IDS.
                var conflictStudyClasses = _studyClassRepo.Query()
                                                          .Include(x => x.Schedule)
                                                          .Include(x => x.Teacher)
                                                          .Include(x => x.StudyCourse)
                                                            .ThenInclude(x => x.Course)
                                                          .Include(x => x.StudySubject)
                                                            .ThenInclude(x => x.Subject)
                                                          .Where(x => x.ScheduleId.HasValue && x.TeacherId.HasValue
                                                                      && conflictScheduleIds.Contains(x.ScheduleId.Value)
                                                                      && request.TeacherIds.Contains(x.TeacherId.Value))
                                                          .ToList();

                // FETCH ALL APPOINTMENT IDS BASED ON THE TEACHER IDS.
                var appointmentIds = _appointmentMemberRepo.Query()
                                                           .Where(x => x.TeacherId.HasValue
                                                                    && request.TeacherIds.Contains(x.TeacherId.Value))
                                                           .Select(x => x.AppointmentId)
                                                           .ToList();

                // FETCH ALL CONFLICTING APPOINTMENTS BASED ON THE APPOINTMENT IDS AND SCHEDULE IDS.
                var conflictAppointments = _appointmentSlotRepo.Query()
                                                               .Include(x => x.Schedule)
                                                               .Where(x => x.ScheduleId.HasValue
                                                                           && appointmentIds.Contains(x.AppointmentId)
                                                                           && conflictScheduleIds.Contains(x.ScheduleId.Value))
                                                               .ToList();

                // GET THE DATES OF CONFLICTING STUDY CLASSES AND APPOINTMENTS.
                conflictDates = conflictStudyClasses.Select(x => x.Schedule.Date)
                                                    .Union(conflictAppointments.Select(x => x.Schedule.Date))
                                                    .Distinct()
                                                    .ToList();

                // GET ALL HOLIDAYS
                var holidayDates = _scheduleRepo.Query()
                                                .Where(x => x.CalendarType == DailyCalendarType.HOLIDAY
                                                            && conflictDates.Contains(x.Date))
                                                .Select(x => x.Date)
                                                .ToList();

                // GROUP STUDY CLASSES BY SCHEDULE ID AND TEACHER ID.
                var groupedStudyClasses = conflictStudyClasses.GroupBy(sc => new { sc.StudyCourseId })
                                                              .Select(group => new
                                                              {
                                                                  group.Key.StudyCourseId,
                                                                  Teachers = group.GroupBy(sc => sc.TeacherId) // Ensure distinct by TeacherId
                                                                                  .Select(g => g.First())
                                                                                  .Select(sc => new TeacherNameResponseDto
                                                                                  {
                                                                                      TeacherId = sc.TeacherId.Value,
                                                                                      FirstName = sc.Teacher.FirstName,
                                                                                      LastName = sc.Teacher.LastName,
                                                                                      Nickname = sc.Teacher.Nickname,
                                                                                  })
                                                                                 .ToList(),
                                                                  ConflictedScheduleIds = group.Distinct().Select(sc => sc.ScheduleId.Value).ToList(),
                                                                  Dates = group.Select(sc => sc.Schedule.Date.ToString("dd MMM yyyy")).Distinct(),
                                                                  group.First().Schedule.FromTime,
                                                                  group.First().Schedule.ToTime,
                                                                  CourseName = group.First().StudyCourse.Course?.course,
                                                                  StudySubjects = group.GroupBy(x => x.StudySubjectId)
                                                                                       .Select(x => x.First())
                                                                                       .Select(x => new ConflictedStudySubjectDTO
                                                                                       {
                                                                                           StudySubjectId = group.First().StudySubjectId.Value,
                                                                                           StudySubjectName = group.First().StudySubject.Subject.subject
                                                                                       })
                                                                                      .ToList()
                                                              })
                                                             .ToList();

                // ADD CONFLICTING STUDY CLASSES' TEACHER DETAILS.
                foreach (var group in groupedStudyClasses)
                {
                    var conflictedClass = new ConflictScheduleDTO
                    {
                        ConflictedTeachers = group.Teachers,
                        Dates = group.Dates,
                        FromTime = group.FromTime,
                        ToTime = group.ToTime,
                        Hour = group.ToTime.Hours - group.FromTime.Hours,
                        StudyCourseId = group.StudyCourseId,
                        StudySubjects = group.StudySubjects,
                        CourseName = group.CourseName,
                        ConflictedScheduleIds = group.ConflictedScheduleIds,
                    };

                    conflictedSchedules.Add(conflictedClass);
                }

                // GROUP CONFLICTING APPOINTMENTS BY SCHEDULE AND TEACHER
                var appointmentMembers = _appointmentMemberRepo.Query()
                                                               .Include(x => x.Teacher)
                                                               .Where(am => conflictAppointments.Select(ca => ca.AppointmentId).Contains(am.AppointmentId))
                                                               .ToList();

                var groupedAppointments = conflictAppointments
                    .GroupBy(x => new { x.AppointmentId })
                    .Select(x => new
                    {
                        x.Key.AppointmentId,
                        Dates = x.Select(ap => ap.Schedule.Date.ToString("dd MMM yyyy")).Distinct(),
                        Teachers = appointmentMembers.Where(am => am.AppointmentId == x.Key.AppointmentId)
                                     .GroupBy(am => am.TeacherId)
                                     .Select(g => g.First())
                                     .Select(am => new TeacherNameResponseDto
                                     {
                                         TeacherId = am.TeacherId.Value,
                                         FirstName = am.Teacher.FirstName,
                                         LastName = am.Teacher.LastName,
                                         Nickname = am.Teacher.Nickname
                                     }).ToList(),
                        x.First().Schedule.FromTime,
                        x.First().Schedule.ToTime,
                        ConflictedScheduleIds = x.Distinct().Select(sc => sc.ScheduleId.Value).ToList(),

                    }).ToList();

                foreach (var group in groupedAppointments)
                {
                    var conflictAppointment = new ConflictScheduleDTO
                    {
                        ConflictedTeachers = group.Teachers,
                        Dates = group.Dates,
                        FromTime = group.FromTime,
                        ToTime = group.ToTime,
                        Hour = group.ToTime.Hours - group.FromTime.Hours,
                        AppointmentId = group.AppointmentId,
                        ConflictedScheduleIds = group.ConflictedScheduleIds
                    };

                    conflictedSchedules.Add(conflictAppointment);
                }
            }

            // FILTER OUT THE CONFLICTING DATES FROM THE REQUEST DATES
            var availableDates = dates.Except(conflictDates).ToList();

            // GENERATE AVAILABLE SCHEDULE FOR THE NON-CONFLICTING DATES
            var availableSchedules = new List<GeneratedAppointmentScheduleDTO>();
            decimal accumulatedHours = 0;

            foreach (var date in availableDates)
            {
                var hour = (decimal)(request.ToTime - request.FromTime).TotalHours;

                var availableSchedule = new GeneratedAppointmentScheduleDTO
                {
                    Id = Guid.NewGuid(),
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
                                                                        x.FromTime < availableSchedule.ToTime &&
                                                                        x.ToTime > availableSchedule.FromTime &&
                                                                        x.AppointmentType == availableSchedule.AppointmentType)
                                                                        ?? false;

                if (!alreadyExists)
                {
                    availableSchedules.Add(availableSchedule);
                }
            }

            var response = new ServiceResponse<AvailableAppointmentDTO>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = new AvailableAppointmentDTO
                {
                    GeneratedSchedules = availableSchedules,
                    ConflictedSchedules = conflictedSchedules
                },
                Success = true
            };

            return response;
        }

        public ServiceResponse<AvailableClassScheduleDTO> GenerateAvailableClassSchedule(CheckAvailableClassScheduleDTO request)
        {
            // QUERY FOR TEACHER TO SEE IF REQUEST IS VALID.
            var teacher = _teacherRepo.Query()
                                      .Include(x => x.Mandays)
                                        .ThenInclude(x => x.WorkTimes)
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
                                                   .Where(x => dates.Contains(x.Date)
                                                            && x.FromTime < request.ToTime && x.ToTime > request.FromTime)
                                                   .Select(x => x.Id)
                                                   .ToList();

            // IF STUDENT IDs IN REQUEST HAS ITEMS, CHECK CONFLICT CLASS FOR STUDENTS.
            List<StudyClass> conflictStudentStudyClasses = new();

            if (request.StudentIds.Any())
            {
                var studentStudyClass = _studyClassRepo.Query()
                                                       .Include(x => x.Schedule)
                                                       .Include(x => x.Teacher)
                                                       .Include(x => x.StudyCourse)
                                                        .ThenInclude(x => x.Course)
                                                       .Include(x => x.StudySubject)
                                                        .ThenInclude(x => x.Subject)
                                                       .Include(x => x.StudySubject)
                                                        .ThenInclude(x => x.StudySubjectMember)
                                                            .ThenInclude(x => x.Student)
                                                       .Where(x => x.ScheduleId.HasValue
                                                                && conflictScheduleIds.Contains(x.ScheduleId.Value)
                                                                && x.StudySubject.StudySubjectMember.Any(x => request.StudentIds.Contains(x.StudentId)))
                                                       .ToList();

                conflictStudentStudyClasses.AddRange(studentStudyClass);
            }

            // FETCH ALL CONFLICTING STUDY CLASSES BASED ON THE SCHEDULE IDS AND TEACHER IDS.
            var conflictTeacherStudyClasses = _studyClassRepo.Query()
                                                             .Include(x => x.Schedule)
                                                             .Include(x => x.Teacher)
                                                             .Include(x => x.StudyCourse)
                                                                .ThenInclude(x => x.Course)
                                                             .Include(x => x.StudySubject)
                                                              .ThenInclude(x => x.Subject)
                                                             .Include(x => x.StudySubject)
                                                              .ThenInclude(x => x.StudySubjectMember)
                                                                  .ThenInclude(x => x.Student)
                                                             .Where(x => x.ScheduleId.HasValue
                                                                      && conflictScheduleIds.Contains(x.ScheduleId.Value)
                                                                      && x.TeacherId == request.TeacherId)
                                                             .ToList();

            // GET CONFLICT STUDY CLASSES.
            List<StudyClass> conflictStudyClasses = new();
            conflictStudyClasses.AddRange(conflictTeacherStudyClasses);

            // JOIN STUDENT'S CONFLICT CLASSES WITH TEACHER'S IF THERE ARE ANY.
            if (conflictStudentStudyClasses.Any())
            {
                conflictStudyClasses.AddRange(conflictStudentStudyClasses);
            }

            if (conflictTeacherStudyClasses.Any())
            {
                conflictStudyClasses.AddRange(conflictTeacherStudyClasses);
            }

            // FETCH ALL CONFLICTING APPOINTMENT IDS BASED ON THE TEACHER IDS.
            var conflictAppointmentIds = _appointmentMemberRepo.Query()
                                                               .Where(x => x.TeacherId == request.TeacherId)
                                                               .Select(x => x.AppointmentId)
                                                               .ToList();

            // FETCH ALL CONFLICTING APPOINTMENTS BASED ON THE APPOINTMENT IDS AND SCHEDULE IDS.
            var conflictAppointments = _appointmentSlotRepo.Query()
                                                           .Include(x => x.Schedule)
                                                           .Where(x => x.ScheduleId.HasValue
                                                                       && conflictAppointmentIds.Contains(x.AppointmentId)
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
            var currentSchedules = request.CurrentSchedules ?? new List<GeneratedAvailableClassScheduleDTO>();

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

            var availableSchedules = new List<GeneratedAvailableClassScheduleDTO>();
            decimal accumulatedHours = 0;

            foreach (var date in availableDates)
            {
                var hour = (decimal)(request.ToTime - request.FromTime).TotalHours;

                var availableSchedule = new GeneratedAvailableClassScheduleDTO
                {
                    Id = Guid.NewGuid(),
                    Teacher = new TeacherNameResponseDto
                    {
                        TeacherId = teacher.Id,
                        FirstName = teacher.FirstName,
                        LastName = teacher.LastName,
                        FullName = teacher.FullName,
                        Nickname = teacher.Nickname,
                    },
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

                var workTypes = _teacherService.GetTeacherWorkTypesWithHours(teacher, date, request.FromTime, request.ToTime);

                foreach (var workType in workTypes)
                {
                    if (workType.TeacherWorkType != TeacherWorkType.NORMAL)
                    {
                        availableSchedule.AdditionalHours = new AdditionalHours
                        {
                            TeacherWorkType = workType.TeacherWorkType,
                            Hours = workType.Hours
                        };
                    }
                }

                availableSchedules.Add(availableSchedule);
            }

            // GET ALL HOLIDAYS
            var holidayDates = _scheduleRepo.Query()
                                            .Where(x => x.CalendarType == DailyCalendarType.HOLIDAY
                                                        && conflictDates.Contains(x.Date))
                                            .Select(x => x.Date)
                                            .ToList();

            var conflictedSchedules = new List<ConflictScheduleDTO>();

            // GROUP STUDY CLASSES BY SCHEDULE ID AND TEACHER ID.
            var groupedStudyClasses = conflictStudyClasses
                .GroupBy(sc => new { sc.StudyCourseId })
                .Select(group => new
                {
                    group.Key.StudyCourseId,
                    Teachers = group.Where(sc => sc.TeacherId.HasValue || sc.StudySubject.StudySubjectMember.Any(ssm => ssm.Student != null))
                                    .GroupBy(sc => sc.TeacherId) // Ensure distinct by TeacherId
                                    .Select(g => g.First())
                                    .Select(sc => new TeacherNameResponseDto
                                    {
                                        TeacherId = sc.TeacherId.Value,
                                        FirstName = sc.Teacher.FirstName,
                                        LastName = sc.Teacher.LastName,
                                        Nickname = sc.Teacher.Nickname,
                                    })
                                   .ToList(),
                    Students = group.Where(sc => sc.StudySubject.StudySubjectMember.Any(ssm => ssm.Student != null) || sc.TeacherId.HasValue)
                                    .SelectMany(sc => sc.StudySubject.StudySubjectMember)
                                    .Where(ssm => ssm.Student != null)
                                    .GroupBy(ssm => ssm.Student.Id) // Group by StudentId to ensure uniqueness
                                    .Select(g => g.First().Student)
                                    .Select(student => new StudentNameResponseDto
                                    {
                                        StudentId = student.Id,
                                        FirstName = student.FirstName,
                                        LastName = student.LastName,
                                        FullName = student.FullName,
                                        Nickname = student.Nickname
                                    })
                                   .ToList(),
                    ConflictedScheduleIds = group.Distinct().Select(sc => sc.ScheduleId.Value).ToList(),
                    Dates = group.Select(sc => sc.Schedule.Date.ToString("dd MMM yyyy")).Distinct(),
                    group.First().Schedule.FromTime,
                    group.First().Schedule.ToTime,
                    CourseName = group.First().StudyCourse.Course?.course,
                    StudySubjects = group.GroupBy(x => x.StudySubjectId)
                                         .Select(x => x.First())
                                         .Select(x => new ConflictedStudySubjectDTO
                                         {
                                             StudySubjectId = group.First().StudySubjectId.Value,
                                             StudySubjectName = group.First().StudySubject.Subject.subject
                                         })
                                        .ToList()
                })
               .ToList();

            // ADD CONFLICTING STUDY CLASSES' TEACHER DETAILS.
            foreach (var group in groupedStudyClasses)
            {
                var conflictedClass = new ConflictScheduleDTO
                {
                    ConflictedTeachers = group.Teachers,
                    ConflictedStudents = group.Students, // Include students
                    Dates = group.Dates,
                    FromTime = group.FromTime,
                    ToTime = group.ToTime,
                    Hour = group.ToTime.Hours - group.FromTime.Hours,
                    StudyCourseId = group.StudyCourseId,
                    StudySubjects = group.StudySubjects,
                    CourseName = group.CourseName,
                    ConflictedScheduleIds = group.ConflictedScheduleIds,
                };

                conflictedSchedules.Add(conflictedClass);
            }

            // GROUP CONFLICTING APPOINTMENTS BY SCHEDULE AND TEACHER
            var groupedAppointments = conflictAppointments
                .GroupBy(x => new { x.AppointmentId })
                .Select(x => new
                {
                    x.Key.AppointmentId,
                    Dates = x.Select(ap => ap.Schedule.Date.ToString("dd MMM yyyy")).Distinct(),
                    Teachers = x.SelectMany(ap => _appointmentMemberRepo.Query()
                                      .Where(am => am.AppointmentId == ap.AppointmentId)
                                      .GroupBy(am => am.TeacherId) // Ensure distinct by TeacherId
                                      .Select(g => g.First())
                                      .Select(am => new TeacherNameResponseDto
                                      {
                                          TeacherId = am.TeacherId.Value,
                                          FirstName = am.Teacher.FirstName,
                                          LastName = am.Teacher.LastName,
                                          Nickname = am.Teacher.Nickname
                                      })).ToList(),
                    x.First().Schedule.FromTime,
                    x.First().Schedule.ToTime,
                    ConflictedScheduleIds = x.Select(sc => sc.ScheduleId.Value).ToList(),

                }).ToList();

            foreach (var group in groupedAppointments)
            {
                var conflictAppointment = new ConflictScheduleDTO
                {
                    ConflictedTeachers = group.Teachers,
                    Dates = group.Dates,
                    FromTime = group.FromTime,
                    ToTime = group.ToTime,
                    Hour = group.ToTime.Hours - group.FromTime.Hours,
                    AppointmentId = group.AppointmentId,
                    ConflictedScheduleIds = group.ConflictedScheduleIds
                };

                conflictedSchedules.Add(conflictAppointment);
            }

            var response = new ServiceResponse<AvailableClassScheduleDTO>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = new AvailableClassScheduleDTO
                {
                    GeneratedSchedules = availableSchedules,
                    ConflictedSchedules = conflictedSchedules
                },
                Success = true
            };

            return response;
        }

        public ServiceResponse<AvailableDTO> CheckAvailableTeacherAppointment(int appointmentId, CheckAvailableTeacherAppointmentDTO request)
        {
            // Get all unique dates from the current schedules
            var scheduleDates = request.CurrentSchedules.Select(x => x.Date).Distinct();
            var dates = scheduleDates.Select(x => x.ToGregorianDateTime()).ToList();

            // Fetch conflicting schedules based on dates
            var conflictSchedules = _scheduleRepo.Query()
                                                 .Where(x => dates.Contains(x.Date))
                                                 .ToList();

            // Fetch all conflicting study classes based on schedule IDs and teacher IDs
            var conflictScheduleIds = conflictSchedules.Select(x => x.Id).ToList();
            var conflictStudyClasses = _studyClassRepo.Query()
                                                      .Include(x => x.Schedule)
                                                      .Where(x => conflictScheduleIds.Contains(x.ScheduleId.Value)
                                                               && request.TeacherIds.Contains(x.TeacherId.Value))
                                                      .ToList();

            // Fetch all conflicting appointments based on teacher IDs
            var appointmentIds = _appointmentMemberRepo.Query()
                                                       .Where(x => request.TeacherIds.Contains(x.TeacherId.Value))
                                                       .Select(x => x.AppointmentId)
                                                       .ToList();

            var conflictAppointments = _appointmentSlotRepo.Query()
                                                           .Include(x => x.Schedule)
                                                           .Where(x => appointmentIds.Contains(x.AppointmentId)
                                                                    && conflictScheduleIds.Contains(x.ScheduleId.Value))
                                                           .ToList();

            // Get all conflicting dates and times
            var conflictDates = new HashSet<DateTime>();
            var conflictDetails = new Dictionary<DateTime, List<string>>();

            foreach (var studyClass in conflictStudyClasses)
            {
                var schedule = studyClass.Schedule;
                if (IsTimeOverlap(request.CurrentSchedules, schedule))
                {
                    conflictDates.Add(schedule.Date);

                    var teacherNickname = _teacherRepo.Query()
                                                      .Where(x => x.Id == studyClass.TeacherId)
                                                      .Select(x => x.Nickname)
                                                      .FirstOrDefault();

                    if (!conflictDetails.ContainsKey(schedule.Date))
                    {
                        conflictDetails[schedule.Date] = new List<string>();
                    }

                    if (!string.IsNullOrEmpty(teacherNickname))
                    {
                        conflictDetails[schedule.Date].Add($"T. {teacherNickname}");
                    }
                }
            }

            foreach (var appointment in conflictAppointments)
            {
                var schedule = appointment.Schedule;
                if (IsTimeOverlap(request.CurrentSchedules, schedule))
                {
                    conflictDates.Add(schedule.Date);

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

                        if (!conflictDetails.ContainsKey(schedule.Date))
                        {
                            conflictDetails[schedule.Date] = new List<string>();
                        }

                        if (!string.IsNullOrEmpty(teacherNickname))
                        {
                            conflictDetails[schedule.Date].Add($"T. {teacherNickname}");
                        }
                    }
                }
            }

            // Get holidays
            var holidayDates = _scheduleRepo.Query()
                                            .Where(x => x.CalendarType == DailyCalendarType.HOLIDAY
                                                     && conflictDates.Contains(x.Date))
                                            .Select(x => x.Date)
                                            .ToList();

            // Format the conflict dates and details
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
            }).ToList();

            string formattedDateString = string.Join(", ", formattedDates);

            string errorMessage = $"There is a scheduling conflict on {formattedDateString}. Please choose a different date or time.";

            if (conflictDates.Any())
            {
                throw new ConflictException(errorMessage);
            }

            var response = new ServiceResponse<AvailableDTO>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = new AvailableDTO
                {
                    IsAvailabled = true
                },
                Success = true
            };

            return response;
        }

        private bool IsTimeOverlap(IEnumerable<GeneratedAppointmentScheduleDTO> requestedSchedules, Schedule schedule)
        {
            var requestedFromTime = requestedSchedules.Select(x => x.FromTime).FirstOrDefault();
            var requestedToTime = requestedSchedules.Select(x => x.ToTime).FirstOrDefault();

            return (requestedFromTime < schedule.ToTime) && (requestedToTime > schedule.FromTime);
        }


        #endregion
    }
}
