using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Google.Api;
using griffined_api.Extensions.DateTimeExtensions;

namespace griffined_api.Services.CheckAvailableService
{
    public class CheckAvailableService : ICheckAvailableService
    {
        private readonly DataContext _context;
        private readonly ITeacherService _teacherService;
        private readonly IMapper _mapper;

        public CheckAvailableService(IMapper mapper, DataContext context, ITeacherService teacherService)
        {
            _mapper = mapper;
            _context = context;
            _teacherService = teacherService;
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

            var dbRequestedStudents = await _context.Students.Where(s => requestedSchedule.StudentIds.Contains(s.Id)).ToListAsync();

            var allCourse = await _context.Courses.Include(c => c.Subjects).Include(c => c.Levels).ToListAsync();

            var requestedStudySubject = dbAllStudySubjects.FirstOrDefault(s => s.Id == requestedSchedule.RequestedStudySubjectId);

            var requestedTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == requestedSchedule.TeacherId)
                                ?? throw new NotFoundException($"Teacher with ID {requestedSchedule.TeacherId} is not found.");

            var dbAppointmentSchedules = await _context.Schedules
                                    .Include(s => s.AppointmentSlot)
                                        .ThenInclude(a => a!.Appointment)
                                            .ThenInclude(a => a.AppointmentMembers)
                                                .ThenInclude(m => m.Teacher)
                                    .Where(s => listRequestedDate.Contains(s.Date) && s.Type == ScheduleType.Appointment
                                    && s.AppointmentSlot!.Appointment.AppointmentMembers.Any(m => m.TeacherId == requestedTeacher.Id)).ToListAsync();

            var conflictSchedule = new List<ConflictScheduleResponseDto>();
            var conflictAppointment = new List<ConflictScheduleResponseDto>();
            var availableSchedule = new List<AvailableScheduleResponseDto>();

            var data = new CheckScheduleResultResponseDto();

            foreach (var requestDate in listRequestedDate)
            {
                foreach (var dbStudyClass in dbStudyClasses.Where(s => s.Schedule.Date == requestDate))
                {
                    if (requestedSchedule.FromTime.ToTimeSpan().TotalMilliseconds < dbStudyClass.Schedule.ToTime.TotalMilliseconds
                        && dbStudyClass.Schedule.FromTime.TotalMilliseconds < requestedSchedule.ToTime.ToTimeSpan().TotalMilliseconds)
                    {
                        var conflict = new ConflictScheduleResponseDto
                        {
                            Message = dbStudyClass.Schedule.Date.ToDateString() + "("
                                            + dbStudyClass.Schedule.FromTime.ToTimeSpanString() + " - " + dbStudyClass.Schedule.ToTime.ToTimeSpanString() + "), "
                                            + dbStudyClass.StudyCourse.StudyCourseType + " Course: " + dbStudyClass.StudyCourse.Course.course + " "
                                            + dbStudyClass.StudySubject.Subject.subject + " " + (dbStudyClass.StudyCourse.Level?.level ?? ""),
                        };

                        foreach (var dbStudent in dbRequestedStudents)
                        {
                            if (dbStudyClass.StudySubject.StudySubjectMember.Any(m => m.StudentId == dbStudent.Id))
                            {
                                conflict.ConflictMembers.Add(new ConflictMemberResponseDto
                                {
                                    Role = "Student",
                                    FirstName = dbStudent.FirstName,
                                    LastName = dbStudent.LastName,
                                    Nickname = dbStudent.Nickname,
                                    FullName = dbStudent.FullName,
                                });
                            }
                        }

                        if (dbStudyClass.TeacherId == requestedTeacher.Id)
                        {
                            conflict.ConflictMembers.Add(new ConflictMemberResponseDto
                            {
                                Role = "Teacher",
                                FirstName = requestedTeacher.FirstName,
                                LastName = requestedTeacher.LastName,
                                FullName = requestedTeacher.FullName,
                                Nickname = requestedTeacher.Nickname,
                            });
                        }

                        if (!conflictSchedule.Contains(conflict))
                        {
                            conflictSchedule.Add(conflict);
                        }
                    }
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


                        var conflict = new ConflictScheduleResponseDto
                        {
                            Message = localSchedule.Date.ToDateTime().ToDateString() + "("
                                            + localSchedule.FromTime.ToTimeSpan().ToTimeSpanString()
                                            + " - " + localSchedule.ToTime.ToTimeSpan().ToTimeSpanString() + "), Current Course: "
                                            + conflictCourse.course + " "
                                            + conflictSubject.subject + " " + (conflictLevel?.level ?? ""),
                        };

                        foreach (var dbStudent in dbRequestedStudents)
                        {
                            conflict.ConflictMembers.Add(new ConflictMemberResponseDto
                            {
                                Role = "Student",
                                FirstName = dbStudent.FirstName,
                                LastName = dbStudent.LastName,
                                Nickname = dbStudent.Nickname,
                                FullName = dbStudent.FullName,
                            });
                        }

                        if (dbTeacher.Id == requestedTeacher.Id)
                        {
                            conflict.ConflictMembers.Add(new ConflictMemberResponseDto
                            {
                                Role = "Teacher",
                                FirstName = requestedTeacher.FirstName,
                                LastName = requestedTeacher.LastName,
                                FullName = requestedTeacher.FullName,
                                Nickname = requestedTeacher.Nickname,
                            });
                        }

                        if (!conflictSchedule.Contains(conflict))
                            conflictSchedule.Add(conflict);
                    }
                }

                foreach (var dbAppointmentSchedule in dbAppointmentSchedules.Where(s => s.Date == requestDate))
                {
                    if (requestedSchedule.FromTime.ToTimeSpan().TotalMilliseconds < dbAppointmentSchedule.ToTime.TotalMilliseconds
                    && dbAppointmentSchedule.FromTime.TotalMilliseconds < requestedSchedule.ToTime.ToTimeSpan().TotalMilliseconds)
                    {
                        if (dbAppointmentSchedule.AppointmentSlot == null)
                            throw new InternalServerException("Something went wrong with Schedule Appointment");

                        var conflict = new ConflictScheduleResponseDto
                        {
                            Message = dbAppointmentSchedule.Date.ToDateString() + "("
                                            + dbAppointmentSchedule.FromTime.ToTimeSpanString() + " - "
                                            + dbAppointmentSchedule.ToTime.ToTimeSpanString() + "), "
                                            + dbAppointmentSchedule.AppointmentSlot.Appointment.AppointmentType
                                            + " Appointment: " + dbAppointmentSchedule.AppointmentSlot.Appointment.Title,
                        };

                        conflict.ConflictMembers.Add(new ConflictMemberResponseDto
                        {
                            Role = "Teacher",
                            FirstName = requestedTeacher.FirstName,
                            LastName = requestedTeacher.LastName,
                            FullName = requestedTeacher.FullName,
                            Nickname = requestedTeacher.Nickname,
                        });

                        if (!conflictAppointment.Contains(conflict))
                            conflictAppointment.Add(conflict);
                    }
                }

                conflictSchedule.AddRange(conflictAppointment);

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
                        TeacherWorkType = _teacherService.FindTeacherWorkType(
                            requestedTeacher,
                            requestDate,
                            requestedSchedule.FromTime.ToTimeSpan(),
                            requestedSchedule.ToTime.ToTimeSpan())
                    });
                }
                else
                {
                    data.IsConflict = true;
                }
            }

            if (data.IsConflict)
                data.ConflictSchedule = conflictSchedule;
            else
                data.AvailableSchedule = availableSchedule;

            var response = new ServiceResponse<CheckScheduleResultResponseDto>
            {
                Data = data,
                StatusCode = (int)HttpStatusCode.OK,
            };

            return response;
        }

        public async Task<ServiceResponse<List<AvailableTeacherResponseDto>>> GetAvailableTeacherForAppointment(int? appointmentId, List<LocalAppointmentRequestDto> appointmentRequestDtos)
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
                                    .Where(s => requestedDate.Contains(s.Date) && s.Type == ScheduleType.Appointment
                                    && s.AppointmentSlot != null
                                    && s.AppointmentSlot.AppointmentId != appointmentId).ToListAsync();

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
                        TeacherId = dbTeacher.Id,
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


        public async Task<ServiceResponse<CheckAppointmentConflictResponseDto>> CheckAppointmentConflict(CheckAppointmentConflictRequestDto requestDto)
        {
            var dbClassSchedules = await _context.Schedules
                                    .Include(s => s.StudyClass)
                                        .ThenInclude(c => c!.Teacher)
                                    .Include(s => s.StudyClass)
                                        .ThenInclude(c => c!.StudySubject)
                                            .ThenInclude(s => s.Subject)
                                    .Include(s => s.StudyClass)
                                        .ThenInclude(c => c!.StudyCourse)
                                            .ThenInclude(s => s.Course)
                                    .Include(s => s.StudyClass)
                                        .ThenInclude(c => c!.StudyCourse)
                                            .ThenInclude(s => s.Level)
                                    .Where(s => requestDto.AppointmentSchedule.Select(a => a.Date.ToDateTime()).Contains(s.Date)).ToListAsync();

            var dbAppointmentSchedules = await _context.Schedules
                                    .Include(s => s.AppointmentSlot)
                                        .ThenInclude(a => a!.Appointment)
                                            .ThenInclude(a => a.AppointmentMembers)
                                                .ThenInclude(m => m.Teacher)
                                    .Where(s => requestDto.AppointmentSchedule.Select(a => a.Date.ToDateTime()).Contains(s.Date)
                                    && s.AppointmentSlot != null
                                    && s.AppointmentSlot.AppointmentId != requestDto.AppointmentId)
                                    .ToListAsync();

            var dbTeachers = await _context.Teachers.Where(t => requestDto.TeacherIds.Contains(t.Id)).ToListAsync();


            var data = new CheckAppointmentConflictResponseDto();
            foreach (var requestSchedule in requestDto.AppointmentSchedule)
            {
                foreach (var dbSchedule in dbClassSchedules.Where(s => s.Date == requestSchedule.Date.ToDateTime()))
                {
                    if (dbSchedule.FromTime < requestSchedule.ToTime.ToTimeSpan()
                        && requestSchedule.FromTime.ToTimeSpan() < dbSchedule.ToTime)
                    {
                        if (dbSchedule.StudyClass != null)
                        {
                            var conflictTeacher = dbTeachers.FirstOrDefault(t => t.Id == dbSchedule.StudyClass?.TeacherId);
                            if (conflictTeacher != null)
                            {
                                var conflictMessage = new ConflictScheduleResponseDto
                                {
                                    Message = dbSchedule.Date.ToDateString() + "("
                                            + dbSchedule.FromTime.ToTimeSpanString() + " - " + dbSchedule.ToTime.ToTimeSpanString() + "), "
                                            + dbSchedule.StudyClass.StudyCourse.StudyCourseType + " Course: " + dbSchedule.StudyClass.StudyCourse.Course.course + " "
                                            + dbSchedule.StudyClass.StudySubject.Subject.subject + " " + (dbSchedule.StudyClass.StudyCourse.Level?.level ?? "")
                                };

                                conflictMessage.ConflictMembers.Add(new ConflictMemberResponseDto
                                {
                                    Role = "Teacher",
                                    FirstName = conflictTeacher.FirstName,
                                    LastName = conflictTeacher.LastName,
                                    Nickname = conflictTeacher.Nickname,
                                    FullName = conflictTeacher.FullName,
                                });
                                if (!data.ConflictMessages.Contains(conflictMessage))
                                {
                                    data.ConflictMessages.Add(conflictMessage);
                                }
                                data.IsConflict = true;
                            }
                        }
                    }
                }

                foreach (var dbSchedule in dbAppointmentSchedules.Where(s => s.Date == requestSchedule.Date.ToDateTime()))
                {
                    if (dbSchedule.AppointmentSlot != null)
                    {
                        var conflictMessage = new ConflictScheduleResponseDto
                        {
                            Message = dbSchedule.Date.ToDateString() + "("
                                        + dbSchedule.FromTime.ToTimeSpanString() + " - "
                                        + dbSchedule.ToTime.ToTimeSpanString() + "), "
                                        + dbSchedule.AppointmentSlot.Appointment.AppointmentType
                                        + " Appointment: " + dbSchedule.AppointmentSlot.Appointment.Title
                        };

                        var conflictTeachers = dbTeachers.Where(t => dbSchedule.AppointmentSlot.Appointment.AppointmentMembers.Select(m => m.TeacherId).Contains(t.Id)).ToList();
                        foreach (var conflictTeacher in conflictTeachers)
                        {
                            conflictMessage.ConflictMembers.Add(new ConflictMemberResponseDto
                            {
                                Role = "Teacher",
                                FirstName = conflictTeacher.FirstName,
                                LastName = conflictTeacher.LastName,
                                Nickname = conflictTeacher.Nickname,
                                FullName = conflictTeacher.FullName,
                            });
                            if (!data.ConflictMessages.Contains(conflictMessage))
                            {
                                data.ConflictMessages.Add(conflictMessage);
                            }
                            data.IsConflict = true;
                        }
                    }
                }

            }

            return new ServiceResponse<CheckAppointmentConflictResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
        }

    }
}