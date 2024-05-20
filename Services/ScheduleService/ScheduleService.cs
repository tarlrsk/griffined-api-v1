using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using griffined_api.Dtos.ScheduleDtos;
using griffined_api.Extensions.DateTimeExtensions;

namespace griffined_api.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;
        private readonly IUnitOfWork _uow;
        private readonly IAsyncRepository<Schedule> _scheduleRepo;
        private readonly IAsyncRepository<StudyClass> _studyClassRepo;
        private readonly IAsyncRepository<AppointmentSlot> _appointmentSlotRepo;
        private readonly IAsyncRepository<AppointmentMember> _appointmentMemberRepo;

        public ScheduleService(DataContext context,
                               IFirebaseService firebaseService,
                               IUnitOfWork uow,
                               IAsyncRepository<Schedule> scheduleRepo,
                               IAsyncRepository<StudyClass> studyClassRepo,
                               IAsyncRepository<AppointmentSlot> appointmentSlotRepo,
                               IAsyncRepository<AppointmentMember> appointmentMemberRepo)
        {
            _context = context;
            _firebaseService = firebaseService;
            _uow = uow;
            _scheduleRepo = scheduleRepo;
            _studyClassRepo = studyClassRepo;
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
                                && c.Status != ClassStatus.Cancelled
                                && c.Status != ClassStatus.Deleted
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
                                && c.Status != ClassStatus.Cancelled
                                && c.Status != ClassStatus.Deleted
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
        public async Task<ServiceResponse<List<DailyCalendarResponseDto>>> GetDailyCalendarForStaff(string requestedDate)
        {
            var dbSchedules = await _context.Schedules
                        .Include(s => s.StudyClass)
                            .ThenInclude(c => c!.Teacher)
                        .Include(s => s.StudyClass)
                            .ThenInclude(c => c!.StudySubject)
                                .ThenInclude(s => s.Subject)
                        .Include(s => s.StudyClass)
                            .ThenInclude(c => c!.StudyCourse)
                                .ThenInclude(s => s.Course)
                        .Include(s => s.StudyClass)
                            .ThenInclude(c => c!.TeacherShifts)
                        .Include(s => s.AppointmentSlot)
                            .ThenInclude(s => s!.Appointment)
                                .ThenInclude(a => a.AppointmentMembers)
                                    .ThenInclude(m => m.Teacher)
                        .Where(s => s.Date == requestedDate.ToDateTime()).ToListAsync();

            var groupedSchedules = new List<TeacherScheduleGroup>();

            foreach (var schedule in dbSchedules)
            {
                List<Teacher> dbTeachers = new();

                var studyClassTeacher = schedule.StudyClass?.Teacher;
                if (studyClassTeacher != null)
                    dbTeachers.Add(studyClassTeacher);

                var appointmentTeacher = schedule.AppointmentSlot?.Appointment.AppointmentMembers.Select(m => m.Teacher).ToList() ?? new List<Teacher>();
                dbTeachers.AddRange(appointmentTeacher);

                foreach (var dbTeacher in dbTeachers)
                {
                    var teacherGroup = groupedSchedules.FirstOrDefault(group => group.Teacher == dbTeacher);

                    if (teacherGroup == null)
                    {
                        teacherGroup = new TeacherScheduleGroup
                        {
                            Teacher = dbTeacher,
                            Schedules = new List<Schedule> { schedule }
                        };
                        groupedSchedules.Add(teacherGroup);
                    }
                    else
                    {
                        teacherGroup.Schedules.Add(schedule);
                    }
                }

            }

            var workingTeachers = new List<DailyCalendarResponseDto>();
            foreach (var groupedSchedule in groupedSchedules)
            {
                var dailyCalendar = new DailyCalendarResponseDto
                {
                    Id = groupedSchedule.Teacher?.Id,
                    TeacherId = groupedSchedule.Teacher?.Id,
                    Teacher = groupedSchedule.Teacher?.Nickname,
                };

                foreach (var schedule in groupedSchedule.Schedules)
                {
                    foreach (var teacherShift in schedule?.StudyClass?.TeacherShifts ?? new List<TeacherShift>())
                    {
                        if (schedule?.StudyClass != null && schedule.StudyClass.Status != ClassStatus.Cancelled && schedule.StudyClass.Status != ClassStatus.Deleted)
                        {
                            switch (teacherShift.TeacherWorkType)
                            {
                                case TeacherWorkType.Overtime:
                                    dailyCalendar.Ot += teacherShift.Hours;
                                    break;
                                case TeacherWorkType.Special:
                                    dailyCalendar.Sp += teacherShift.Hours;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                };

                for (int i = 8; i < 20; i++)
                {
                    var firstHalf = TimeSpan.FromHours(i);
                    var secondHalf = firstHalf.Add(TimeSpan.FromMinutes(30));
                    var endHour = secondHalf.Add(TimeSpan.FromMinutes(30));
                    DailyCalendarHalfSlotResponseDto? hourSlot = null;
                    foreach (var schedule in groupedSchedule.Schedules)
                    {
                        if (schedule.FromTime < secondHalf
                        && firstHalf < schedule.ToTime)
                        {
                            if (hourSlot == null)
                            {
                                hourSlot = new DailyCalendarHalfSlotResponseDto
                                {
                                    FirstHalf = new DailyCalendarSlotResponseDto
                                    {
                                        ScheduleId = schedule.Id,
                                        Time = $"{schedule.FromTime.ToTimeSpanString()}-{schedule.ToTime.ToTimeSpanString()}",
                                    },
                                };
                                if (schedule.Type == ScheduleType.Appointment)
                                {
                                    hourSlot.FirstHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                    if (schedule.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
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
                                    hourSlot.FirstHalf.Type = schedule.StudyClass!.Status switch
                                    {
                                        ClassStatus.Deleted => DailyCalendarType.CANCELLED_CLASS,
                                        ClassStatus.Cancelled => DailyCalendarType.CANCELLED_CLASS,
                                        _ => DailyCalendarType.NORMAL_CLASS,
                                    };

                                    if (schedule.StudyClass!.IsMakeup)
                                    {
                                        hourSlot.FirstHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                    }

                                    hourSlot.FirstHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                    hourSlot.FirstHalf.Room = schedule.StudyClass.Schedule.Room;
                                }
                            }
                            else
                            {
                                if (hourSlot.FirstHalf?.Type == DailyCalendarType.CANCELLED_CLASS || hourSlot.FirstHalf == null)
                                {
                                    hourSlot.FirstHalf = new DailyCalendarSlotResponseDto
                                    {
                                        ScheduleId = schedule.Id,
                                        Time = $"{schedule.FromTime.ToTimeSpanString()}-{schedule.ToTime.ToTimeSpanString()}",
                                    };

                                    if (schedule.Type == ScheduleType.Appointment)
                                    {
                                        hourSlot.FirstHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                        if (schedule.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
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
                                        hourSlot.FirstHalf.Type = schedule.StudyClass!.Status switch
                                        {
                                            ClassStatus.Deleted => DailyCalendarType.CANCELLED_CLASS,
                                            ClassStatus.Cancelled => DailyCalendarType.CANCELLED_CLASS,
                                            _ => DailyCalendarType.NORMAL_CLASS,
                                        };

                                        if (schedule.StudyClass!.IsMakeup)
                                        {
                                            hourSlot.FirstHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                        }

                                        hourSlot.FirstHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                        hourSlot.FirstHalf.Room = schedule.StudyClass.Schedule.Room;
                                    }
                                }
                            }
                        }
                        if (schedule.FromTime < endHour
                        && secondHalf < schedule.ToTime)
                        {
                            if (hourSlot == null)
                            {
                                hourSlot = new DailyCalendarHalfSlotResponseDto
                                {
                                    SecondHalf = new DailyCalendarSlotResponseDto
                                    {
                                        ScheduleId = schedule.Id,
                                        Time = $"{schedule.FromTime.ToTimeSpanString()}-{schedule.ToTime.ToTimeSpanString()}",
                                    },
                                };
                                if (schedule.Type == ScheduleType.Appointment)
                                {
                                    hourSlot.SecondHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                    if (schedule.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
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
                                    hourSlot.SecondHalf.Type = schedule.StudyClass!.Status switch
                                    {
                                        ClassStatus.Deleted => DailyCalendarType.CANCELLED_CLASS,
                                        ClassStatus.Cancelled => DailyCalendarType.CANCELLED_CLASS,
                                        _ => DailyCalendarType.NORMAL_CLASS,
                                    };

                                    if (schedule.StudyClass!.IsMakeup)
                                    {
                                        hourSlot.SecondHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                    }

                                    hourSlot.SecondHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                    hourSlot.SecondHalf.Room = schedule.StudyClass.Schedule.Room;
                                }
                            }
                            else
                            {
                                if (hourSlot.SecondHalf?.Type == DailyCalendarType.CANCELLED_CLASS || hourSlot.SecondHalf == null)
                                {
                                    hourSlot.SecondHalf = new DailyCalendarSlotResponseDto
                                    {
                                        ScheduleId = schedule.Id,
                                        Time = $"{schedule.FromTime.ToTimeSpanString()}-{schedule.ToTime.ToTimeSpanString()}",
                                    };

                                    if (schedule.Type == ScheduleType.Appointment)
                                    {
                                        hourSlot.SecondHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                        if (schedule.AppointmentSlot.Appointment.AppointmentType == AppointmentType.HOLIDAY)
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
                                        hourSlot.SecondHalf.Type = schedule.StudyClass!.Status switch
                                        {
                                            ClassStatus.Deleted => DailyCalendarType.CANCELLED_CLASS,
                                            ClassStatus.Cancelled => DailyCalendarType.CANCELLED_CLASS,
                                            _ => DailyCalendarType.NORMAL_CLASS,
                                        };

                                        if (schedule.StudyClass!.IsMakeup)
                                        {
                                            hourSlot.SecondHalf.Type = DailyCalendarType.MAKEUP_CLASS;
                                        }

                                        hourSlot.SecondHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                        hourSlot.SecondHalf.Room = schedule.StudyClass.Schedule.Room;

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
            var data = new List<DailyCalendarResponseDto>();
            var teachers = await _context.Teachers.Where(t => t.IsActive == true).ToListAsync();
            foreach (var teacher in teachers)
            {
                var workTeacher = workingTeachers.Where(t => t.Id == teacher.Id).FirstOrDefault();
                if (workTeacher != null)
                {
                    data.Add(workTeacher);
                }
                else
                {
                    data.Add(new DailyCalendarResponseDto()
                    {
                        Id = teacher.Id,
                        Teacher = teacher.Nickname,
                        TeacherId = teacher.Id,
                    });
                }
            }

            return new ServiceResponse<List<DailyCalendarResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data.OrderBy(t => t.Teacher).ToList(),
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

        public ServiceResponse<IEnumerable<AvailableAppointmentScheduleDTO>> GenerateAvailableSchedule(CheckAvailableAppointmentScheduleDTO request)
        {
            // FETCH ALL CONFLICTING SCHEDULE IDS BASED ON THE GIVEN DATES.
            var conflictScheduleIds = _scheduleRepo.Query()
                                                   .Where(x => request.Dates.Contains(x.Date))
                                                   .Select(x => x.Id)
                                                   .ToList();

            // FETCH ALL CONFLICTING STUDY CLASSES BASED ON THE SCHEDULE IDS AND TEACHER IDS.
            var conflictStudyClasses = _studyClassRepo.Query()
                                                      .Include(x => x.Schedule)
                                                      .Where(x => conflictScheduleIds.Contains(x.ScheduleId.Value)
                                                                  && request.TeacherIds.Contains(x.TeacherId.Value))
                                                      .ToList();

            // FETCH ALL CONFLICTING APPOINTMENT IDS BASED ON THE TEACHER IDS.
            var conflictAppointmentIds = _appointmentMemberRepo.Query()
                                                               .Where(x => request.TeacherIds.Contains(x.TeacherId.Value))
                                                               .Select(x => x.AppointmentId)
                                                               .ToList();

            // FETCH ALL CONFLICTING APPOINTMENTS BASED ON THE APPOINTMENT IDS AND SCHEDULE IDS.
            var conflictAppointments = _appointmentSlotRepo.Query()
                                                           .Include(x => x.Schedule)
                                                           .Where(x => conflictAppointmentIds.Contains(x.AppointmentId)
                                                                       && conflictScheduleIds.Contains(x.ScheduleId.Value))
                                                           .ToList();

            // GET THE DATES OF CONFLICTING STUDY CLASSES AND APPOINTMENTS.
            var conflictDates = conflictStudyClasses.Select(x => x.Schedule.Date)
                                                    .Union(conflictAppointments.Select(x => x.Schedule.Date))
                                                    .Distinct()
                                                    .ToList();

            // FILTER OUT THE CONFLICTING DATES FROM THE REQUEST DATES
            var availableDates = request.Dates.Except(conflictDates).ToList();


            // GENERATE AVAILABLE SCHEDULE FOR THE NON-CONFLICTING DATES
            var availableSchedules = new List<AvailableAppointmentScheduleDTO>();
            decimal accumulatedHours = 0;

            foreach (var date in availableDates)
            {
                var hour = (decimal)(request.ToTime - request.FromTime).TotalHours;

                var availableSchedule = new AvailableAppointmentScheduleDTO
                {
                    Date = date,
                    Day = date.DayOfWeek,
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

            var response = new ServiceResponse<IEnumerable<AvailableAppointmentScheduleDTO>>
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