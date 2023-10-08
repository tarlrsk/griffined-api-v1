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
        public ScheduleService(DataContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
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
                                    && a.AppointmentSlotStatus != AppointmentSlotStatus.Deleted
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
                        Room = dbStudyClass.Room,
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

            var data = new List<DailyCalendarResponseDto>();
            foreach (var groupedSchedule in groupedSchedules)
            {
                var dailyCalendar = new DailyCalendarResponseDto
                {
                    Id = groupedSchedule.Teacher?.Id,
                    TeacherId = groupedSchedule.Teacher?.Id,
                    Teacher = groupedSchedule.Teacher?.Nickname,
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
                                    },
                                };
                                if (schedule.Type == ScheduleType.Appointment)
                                {
                                    hourSlot.FirstHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                    hourSlot.FirstHalf.Type = DailyCalendarType.OfficeHours;
                                }
                                else
                                {
                                    hourSlot.FirstHalf.Type = schedule.StudyClass!.Status switch
                                    {
                                        ClassStatus.Deleted => DailyCalendarType.CancelledClass,
                                        ClassStatus.Cancelled => DailyCalendarType.CancelledClass,
                                        _ => DailyCalendarType.StudyClass,
                                    };

                                    if (schedule.StudyClass!.IsMakeup)
                                    {
                                        hourSlot.FirstHalf.Type = DailyCalendarType.MakeupStudyClass;
                                    }

                                    hourSlot.FirstHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                    hourSlot.FirstHalf.Room = schedule.StudyClass.Room;
                                }
                            }
                            else
                            {
                                if (hourSlot.FirstHalf?.Type == DailyCalendarType.CancelledClass || hourSlot.FirstHalf == null)
                                {
                                    hourSlot.FirstHalf = new DailyCalendarSlotResponseDto
                                    {
                                        ScheduleId = schedule.Id,
                                    };

                                    if (schedule.Type == ScheduleType.Appointment)
                                    {
                                        hourSlot.FirstHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                        hourSlot.FirstHalf.Type = DailyCalendarType.OfficeHours;
                                    }
                                    else
                                    {
                                        hourSlot.FirstHalf.Type = schedule.StudyClass!.Status switch
                                        {
                                            ClassStatus.Deleted => DailyCalendarType.CancelledClass,
                                            ClassStatus.Cancelled => DailyCalendarType.CancelledClass,
                                            _ => DailyCalendarType.StudyClass,
                                        };

                                        if (schedule.StudyClass!.IsMakeup)
                                        {
                                            hourSlot.FirstHalf.Type = DailyCalendarType.MakeupStudyClass;
                                        }

                                        hourSlot.FirstHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                        hourSlot.FirstHalf.Room = schedule.StudyClass.Room;
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
                                    },
                                };
                                if (schedule.Type == ScheduleType.Appointment)
                                {
                                    hourSlot.SecondHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                    hourSlot.SecondHalf.Type = DailyCalendarType.OfficeHours;
                                }
                                else
                                {
                                    hourSlot.SecondHalf.Type = schedule.StudyClass!.Status switch
                                    {
                                        ClassStatus.Deleted => DailyCalendarType.CancelledClass,
                                        ClassStatus.Cancelled => DailyCalendarType.CancelledClass,
                                        _ => DailyCalendarType.StudyClass,
                                    };

                                    if (schedule.StudyClass!.IsMakeup)
                                    {
                                        hourSlot.SecondHalf.Type = DailyCalendarType.MakeupStudyClass;
                                    }

                                    hourSlot.SecondHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                    hourSlot.SecondHalf.Room = schedule.StudyClass.Room;
                                }
                            }
                            else
                            {
                                if (hourSlot.SecondHalf?.Type == DailyCalendarType.CancelledClass || hourSlot.SecondHalf == null)
                                {
                                    hourSlot.SecondHalf = new DailyCalendarSlotResponseDto
                                    {
                                        ScheduleId = schedule.Id,
                                    };

                                    if (schedule.Type == ScheduleType.Appointment)
                                    {
                                        hourSlot.SecondHalf.Name = schedule.AppointmentSlot!.Appointment.AppointmentType.ToString();
                                        hourSlot.SecondHalf.Type = DailyCalendarType.OfficeHours;
                                    }
                                    else
                                    {
                                        hourSlot.SecondHalf.Type = schedule.StudyClass!.Status switch
                                        {
                                            ClassStatus.Deleted => DailyCalendarType.CancelledClass,
                                            ClassStatus.Cancelled => DailyCalendarType.CancelledClass,
                                            _ => DailyCalendarType.StudyClass,
                                        };

                                        if (schedule.StudyClass!.IsMakeup)
                                        {
                                            hourSlot.SecondHalf.Type = DailyCalendarType.MakeupStudyClass;
                                        }

                                        hourSlot.SecondHalf.Name = schedule.StudyClass!.StudyCourse.Course.course;
                                        hourSlot.SecondHalf.Room = schedule.StudyClass.Room;
                                    }
                                }
                            }
                        }
                    }
                    dailyCalendar.HourSlots.Add(hourSlot);
                }
                data.Add(dailyCalendar);
            }

            return new ServiceResponse<List<DailyCalendarResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
        }
    }
}