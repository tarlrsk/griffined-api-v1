using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Api;
using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Extensions.DateTimeExtensions;

namespace griffined_api.Services.CheckAvailableService
{
    public class CheckAvailableService : ICheckAvailableService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CheckAvailableService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<CheckScheduleResultResponseDto>> GetAvailableSchedule(RequestedScheduleRequestDto requestedSchedule)
        {
            var listRequestedDate = requestedSchedule.RequestedDates.Select(s => s.ToDateTime()).ToList();

            var dbAllStudySubjects = await _context.StudySubjects
                                    .Include(s => s.Subject)
                                    .Include(s => s.StudyCourse)
                                        .ThenInclude(c => c.Course)
                                    .Include(s => s.StudyCourse)
                                        .ThenInclude(c => c.Level)
                                    .ToListAsync();

            var dbAllTeachers = await _context.Teachers.ToListAsync();

            var dbStudyClasses = await _context.StudyClasses
                            .Include(c => c.StudySubject)
                                .ThenInclude(s => s.StudyCourse)
                                    .ThenInclude(c => c.Course)
                            .Include(c => c.StudySubject)
                                .ThenInclude(s => s.StudyCourse)
                                    .ThenInclude(c => c.Level)
                            .Include(c => c.StudySubject)
                                .ThenInclude(s => s.Subject)
                            .Include(c => c.StudySubject)
                                .ThenInclude(s => s.StudySubjectMember)
                            .Include(c => c.Teacher)
                            .Include(c => c.Schedule)
                            .Where(c => listRequestedDate.Contains(c.Schedule.Date) &&
                            !requestedSchedule.CurrentStudySubjectId.Contains(c.StudySubject.Id)
                            && (c.TeacherId == requestedSchedule.TeacherId
                            || c.StudySubject.StudySubjectMember.Any(member => requestedSchedule.StudentIds.Contains(member.StudentId)))
                            && (c.Status == ClassStatus.None
                            || c.Status == ClassStatus.PendingCancellation))
                            .ToListAsync();

            var dbAppointmentSchedules = await _context.Schedules
                                    .Include(s => s.AppointmentSlot)
                                        .ThenInclude(a => a!.Appointment)
                                            .ThenInclude(a => a.AppointmentMembers)
                                                .ThenInclude(m => m.Teacher)
                                    .Where(s => listRequestedDate.Contains(s.Date) && s.Type == ScheduleType.Appointment).ToListAsync();


            var allCourse = await _context.Courses.Include(c => c.Subjects).Include(c => c.Levels).ToListAsync();

            var requestedStudySubject = dbAllStudySubjects.FirstOrDefault(s => s.Id == requestedSchedule.RequestedStudySubjectId);

            var requestedTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == requestedSchedule.TeacherId)
                                ?? throw new NotFoundException($"Teacher with ID {requestedSchedule.TeacherId} is not found.");

            var conflictSchedule = new List<ConflictClassResponseDto>();
            var availableSchedule = new List<AvailableScheduleResponseDto>();

            var data = new CheckScheduleResultResponseDto();

            foreach (var requestDate in listRequestedDate)
            {
                foreach (var dbStudyClass in dbStudyClasses.Where(s => s.Schedule.Date == requestDate))
                {
                    if (requestedSchedule.FromTime.ToTimeSpan().TotalMilliseconds < dbStudyClass.Schedule.ToTime.TotalMilliseconds
                        && dbStudyClass.Schedule.FromTime.TotalMilliseconds < requestedSchedule.ToTime.ToTimeSpan().TotalMilliseconds)
                    {
                        var conflict = new ConflictClassResponseDto
                        {
                            StudyCourseId = dbStudyClass.StudySubject.StudyCourse.Id,
                            CourseId = dbStudyClass.StudySubject.StudyCourse.Course.Id,
                            Course = dbStudyClass.StudySubject.StudyCourse.Course.course,
                            StudySubjectId = dbStudyClass.StudySubject.Id,
                            SubjectId = dbStudyClass.StudySubject.Subject.Id,
                            Subject = dbStudyClass.StudySubject.Subject.subject,
                            LevelId = dbStudyClass.StudyCourse.Level?.Id,
                            Level = dbStudyClass.StudyCourse.Level?.level,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                            ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                            TeacherId = dbStudyClass.Teacher.Id,
                            TeacherFirstName = dbStudyClass.Teacher.FirstName,
                            TeacherLastName = dbStudyClass.Teacher.LastName,
                            TeacherNickname = dbStudyClass.Teacher.Nickname,
                            // TODO Teacher WorkType
                        };
                        if (!conflictSchedule.Contains(conflict))
                        {
                            conflictSchedule.Add(conflict);
                        }
                    }
                }

                foreach (var dbAppointmentSchedule in dbAppointmentSchedules.Where(s => s.Date == requestDate))
                {
                    
                }

                foreach (var localSchedule in requestedSchedule.LocalSchedule.Where(s => s.Date.ToDateTime() == requestDate))
                {
                    if (requestedSchedule.FromTime.ToTimeSpan().TotalMilliseconds < localSchedule.ToTime.ToTimeSpan().TotalMilliseconds
                    && localSchedule.FromTime.ToTimeSpan().TotalMilliseconds < requestedSchedule.ToTime.ToTimeSpan().TotalMilliseconds)
                    {
                        var dbStudySubject = dbAllStudySubjects.FirstOrDefault(s => s.Id == localSchedule.StudySubjectId);

                        var dbTeacher = dbAllTeachers.FirstOrDefault(t => t.Id == localSchedule.TeacherId)
                                        ?? throw new NotFoundException("TeacherId in LocalSchedule is not found");

                        var conflictCourse = allCourse.FirstOrDefault(c => c.Id == localSchedule.CourseId) ?? throw new NotFoundException("Conflict Course ID is not found.");
                        var conflictSubject = conflictCourse.Subjects.FirstOrDefault(c => c.Id == localSchedule.SubjectId) ?? throw new NotFoundException("Conflict Subject ID is not found.");
                        var conflictLevel = conflictCourse.Levels.FirstOrDefault(c => c.Id == localSchedule.LevelId);


                        var conflict = new ConflictClassResponseDto
                        {
                            StudyCourseId = dbStudySubject?.StudyCourse.Id,
                            CourseId = conflictCourse.Id,
                            Course = conflictCourse.course,
                            StudySubjectId = dbStudySubject?.Id,
                            SubjectId = conflictSubject.Id,
                            Subject = conflictSubject.subject,
                            LevelId = conflictLevel?.Id,
                            Level = conflictLevel?.level,
                            Date = localSchedule.Date.ToDateTime().ToDateString(),
                            FromTime = localSchedule.FromTime.ToTimeSpan().ToTimeSpanString(),
                            ToTime = localSchedule.ToTime.ToTimeSpan().ToTimeSpanString(),
                            TeacherId = dbTeacher.Id,
                            TeacherFirstName = dbTeacher.FirstName,
                            TeacherLastName = dbTeacher.LastName,
                            TeacherNickname = dbTeacher.Nickname,
                            // TODO Teacher WorkType
                        };

                        if (!conflictSchedule.Contains(conflict))
                            conflictSchedule.Add(conflict);
                    }
                }

                var availableCourse = allCourse.FirstOrDefault(c => c.Id == requestedSchedule.RequestedCourseId) ?? throw new NotFoundException("Course ID is not found");
                var availableSubject = availableCourse.Subjects.FirstOrDefault(c => c.Id == requestedSchedule.RequestedSubjectId) ?? throw new NotFoundException("Subject ID is not found");
                var availableLevel = availableCourse.Levels.FirstOrDefault(c => c.Id == requestedSchedule.RequestedLevelId);

                if (conflictSchedule.IsNullOrEmpty())
                {
                    availableSchedule.Add(new AvailableScheduleResponseDto
                    {
                        StudyCourseId = requestedStudySubject?.StudyCourse.Id,
                        CourseId = availableCourse.Id,
                        Course = availableCourse.course,
                        StudySubjectId = requestedStudySubject?.Id,
                        SubjectId = availableSubject.Id,
                        Subject = availableSubject.subject,
                        LevelId = availableLevel?.Id,
                        Level = availableLevel?.level,
                        Date = requestDate.ToDateString(),
                        FromTime = requestedSchedule.FromTime,
                        ToTime = requestedSchedule.ToTime,
                        TeacherId = requestedTeacher.Id,
                        TeacherFirstName = requestedTeacher.FirstName,
                        TeacherLastName = requestedTeacher.LastName,
                        TeacherNickname = requestedTeacher.Nickname,
                        // TODO Teacher WorkType
                    });
                }
                else
                {
                    data.IsConflict = true;
                }
            }

            if (data.IsConflict)
                data.ConflictClasses = conflictSchedule;
            else
                data.AvailableSchedule = availableSchedule;

            var response = new ServiceResponse<CheckScheduleResultResponseDto>
            {
                Data = data,
                StatusCode = (int)HttpStatusCode.OK,
            };

            return response;
        }

        public async Task<ServiceResponse<List<AvailableTeacherResponseDto>>> GetAvailableTeacherForAppointment(List<LocalAppointmentRequestDto> appointmentRequestDtos)
        {
            var requestedDate = appointmentRequestDtos.Select(a => a.Date.ToDateTime()).ToList();

            var dbClassSchedules = await _context.Schedules
                                    .Include(s => s.StudyClass)
                                    .Where(s => requestedDate.Contains(s.Date) && s.Type == ScheduleType.Class).ToListAsync();

            var dbAppointmentSchedules = await _context.Schedules
                                    .Include(s => s.AppointmentSlot)
                                        .ThenInclude(a => a!.Appointment)
                                            .ThenInclude(a => a.AppointmentMembers)
                                                .ThenInclude(m => m.Teacher)
                                    .Where(s => requestedDate.Contains(s.Date) && s.Type == ScheduleType.Appointment).ToListAsync();

            var dbTeachers = await _context.Teachers
                                .Include(t => t.WorkTimes)
                                .Where(t => t.IsActive == true)
                                .ToListAsync();

            List<AvailableTeacherResponseDto> data = new();

            foreach (var dbTeacher in dbTeachers)
            {
                var isConflict = false;
                foreach (var appointmentRequest in appointmentRequestDtos)
                {
                    foreach (var dbClassSchedule in dbClassSchedules.Where(s => s.Date == appointmentRequest.Date.ToDateTime()))
                    {
                        if (appointmentRequest.FromTime.ToTimeSpan().TotalMilliseconds < dbClassSchedule.ToTime.TotalMilliseconds
                        && dbClassSchedule.FromTime.TotalMilliseconds < appointmentRequest.ToTime.ToTimeSpan().TotalMilliseconds
                        && dbClassSchedule.StudyClass?.TeacherId == dbTeacher.Id)
                        {
                            isConflict = true;
                            break;
                        }
                    }

                    if (isConflict == true)
                    {
                        break;
                    }

                    foreach (var dbAppointmentSchedule in dbAppointmentSchedules.Where(s => s.Date == appointmentRequest.Date.ToDateTime()))
                    {
                        if (appointmentRequest.FromTime.ToTimeSpan().TotalMilliseconds < dbAppointmentSchedule.ToTime.TotalMilliseconds
                        && dbAppointmentSchedule.FromTime.TotalMilliseconds < appointmentRequest.ToTime.ToTimeSpan().TotalMilliseconds
                        && dbAppointmentSchedule.AppointmentSlot?.Appointment?.AppointmentMembers?.Any(m => m.TeacherId == dbTeacher.Id) == true)
                        {
                            isConflict = true;
                            break;
                        }
                    }
                }
                if (isConflict == false)
                {
                    data.Add(new AvailableTeacherResponseDto
                    {
                        Id = dbTeacher.Id,
                        FirebaseId = dbTeacher.FirebaseId,
                        FirstName = dbTeacher.FirstName,
                        LastName = dbTeacher.LastName,
                        FullName = dbTeacher.FullName,
                        Nickname = dbTeacher.Nickname,
                        // TODO Teacher WorkType
                    });
                }
            }

            return new ServiceResponse<List<AvailableTeacherResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
        }

        // public async Task<ServiceResponse<List<GetAvailableTeacherDto>>> GetAvailableTeacher(string fromTime, string toTime, string date, int classId)
        // {
        //     var response = new ServiceResponse<List<GetAvailableTeacherDto>>();
        //     DateTime dateParse = DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
        //     date = dateParse.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //     TimeSpan duration = DateTime.Parse(toTime).Subtract(DateTime.Parse(fromTime));
        //     string _day = dateParse.DayOfWeek.ToString();

        //     var dbTeachers = await _context.Teachers
        //         .Include(t => t.workTimes)
        //         .Select(t => _mapper.Map<GetTeacherDto>(t))
        //         .ToListAsync();

        //     var availableTeacher = new List<GetAvailableTeacherDto>();

        //     var preferFromTime = TimeOnly.Parse(fromTime);
        //     var preferToTime = TimeOnly.Parse(toTime);

        //     foreach (var teacher in dbTeachers)
        //     {
        //         var conflictStatus = false;
        //         var _currentClass = false;
        //         var cls = await _context.PrivateClasses.Where(pc => 
        //                 pc.teacherPrivateClass != null && 
        //                 pc.teacherPrivateClass.teacherId == teacher.id
        //                 && pc.date == date && pc.isActive == true).ToListAsync();
        //         if (cls != null)
        //         {
        //             foreach (var c in cls)
        //             {
        //                 TimeSpan c_duration = DateTime.Parse(c.toTime).Subtract(DateTime.Parse(c.fromTime));
        //                 var c_fromTime = TimeOnly.Parse(c.fromTime);
        //                 var c_toTime = TimeOnly.Parse(c.toTime);
        //                 if (c.id == classId && duration == c_duration)
        //                 {
        //                     _currentClass = true;
        //                 }
        //                 else if (c.id != classId)
        //                 {
        //                     if (!(preferFromTime >= c_toTime || c_fromTime >= preferToTime))
        //                     {
        //                         conflictStatus = true;
        //                         break;
        //                     }
        //                 }
        //             }
        //         }
        //         var workDay = teacher.workTimes.FirstOrDefault(w => w.day == _day.ToLower());

        //         if (conflictStatus != true && teacher.isActive == true)
        //         {
        //             if (workDay == null)
        //             {
        //                 availableTeacher.Add(new GetAvailableTeacherDto
        //                 {
        //                     id = teacher.id,
        //                     firebaseId = teacher.firebaseId,
        //                     fName = teacher.fName,
        //                     lName = teacher.lName,
        //                     nickname = teacher.nickname,
        //                     workType = "SP",
        //                     currentClass = _currentClass
        //                 });
        //             }
        //             else if ((preferFromTime >= TimeOnly.Parse(workDay.toTime) || TimeOnly.Parse(workDay.fromTime) >= preferToTime))
        //             {
        //                 availableTeacher.Add(new GetAvailableTeacherDto
        //                 {
        //                     id = teacher.id,
        //                     firebaseId = teacher.firebaseId,
        //                     fName = teacher.fName,
        //                     lName = teacher.lName,
        //                     nickname = teacher.nickname,
        //                     workType = "OT",
        //                     currentClass = _currentClass
        //                 });
        //             }
        //             else
        //             {
        //                 availableTeacher.Add(new GetAvailableTeacherDto
        //                 {
        //                     id = teacher.id,
        //                     firebaseId = teacher.firebaseId,
        //                     fName = teacher.fName,
        //                     lName = teacher.lName,
        //                     nickname = teacher.nickname,
        //                     workType = "Normal",
        //                     currentClass = _currentClass
        //                 });
        //             }

        //         };
        //     }
        //     response.Data = availableTeacher;
        //     return response;
        // }

        // public async Task<ServiceResponse<List<GetAvailableTimeDto>>> GetAvailableTime([FromQuery] int[] listOfStudentId, string date, int hour, int classId)
        // {
        //     var response = new ServiceResponse<List<GetAvailableTimeDto>>();
        //     var dateParse = DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
        //     date = dateParse.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //     var fixPeriod = CreateFixPeriod(hour);
        //     var conflicts = new List<GetConflictTimeDto>();
        //     foreach (var sId in listOfStudentId)
        //     {
        //         var student = await _context.Students.FindAsync(sId);
        //         if (student is null)
        //             throw new Exception($"Student with ID {sId} not found.");

        //         var cls = await _context.PrivateClasses.Where(pc => pc.isActive == true && pc.studentPrivateClasses!
        //                 .Any(spc => spc.studentId == sId)
        //                 && pc.date == date).ToListAsync();
        //         if (cls != null)
        //         {
        //             foreach (var c in cls)
        //             {
        //                 TimeSpan duration = DateTime.Parse(c.toTime).Subtract(DateTime.Parse(c.fromTime));
        //                 if (c.id == classId && duration.Hours == hour)
        //                 {
        //                     conflicts.Add(new GetConflictTimeDto
        //                     {
        //                         id = c.id,
        //                         fromTime = c.fromTime,
        //                         toTime = c.toTime,
        //                         currentClass = true
        //                     });
        //                 }
        //                 else if (c.id != classId)
        //                 {
        //                     conflicts.Add(new GetConflictTimeDto
        //                     {
        //                         id = c.id,
        //                         fromTime = c.fromTime,
        //                         toTime = c.toTime,
        //                         currentClass = false
        //                     });

        //                 }
        //             }
        //         }
        //     };
        //     conflicts = conflicts.DistinctBy(i => i.id).ToList();

        //     var availableTimes = new List<GetAvailableTimeDto>();
        //     foreach (var f in fixPeriod)
        //     {
        //         var f_fromTime = TimeOnly.Parse(f.fromTime);
        //         var f_toTime = TimeOnly.Parse(f.toTime);
        //         var _currentClass = false;
        //         var conflictStatus = false;
        //         foreach (var c in conflicts)
        //         {
        //             var c_fromTime = TimeOnly.Parse(c.fromTime);
        //             var c_toTime = TimeOnly.Parse(c.toTime);

        //             if (!(f_fromTime >= c_toTime || c_fromTime >= f_toTime) && c.currentClass == false)
        //             {
        //                 conflictStatus = true;
        //                 break;
        //             }
        //             else if (!(f_fromTime >= c_toTime || c_fromTime >= f_toTime) && c.currentClass == true)
        //             {
        //                 _currentClass = true;
        //             }
        //         }
        //         if (conflictStatus == false)
        //         {
        //             availableTimes.Add(new GetAvailableTimeDto
        //             {
        //                 fromTime = f.fromTime,
        //                 toTime = f.toTime,
        //                 currentClass = _currentClass
        //             });
        //         }
        //     }
        //     response.Data = availableTimes;

        //     return response;
        // }
        private List<AvailableTimeResponseDto> CreateFixPeriod(int hour)
        {
            var fixPeriod = new List<AvailableTimeResponseDto>();
            var startTime = TimeOnly.Parse("10:00");
            var endTime = TimeOnly.Parse("20:00").AddHours(-(hour));
            while (startTime <= endTime)
            {
                fixPeriod.Add(new AvailableTimeResponseDto { FromTime = startTime.ToString(), ToTime = (startTime.AddHours(hour)).ToString() });
                startTime = startTime.AddHours(1);
            }
            return fixPeriod;
        }
    }
}