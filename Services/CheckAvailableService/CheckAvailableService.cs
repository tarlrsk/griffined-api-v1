using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoMapper.Execution;
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
                            .Include(c => c.StudyCourse)
                                .ThenInclude(c => c.Course)
                            .Include(c => c.Teacher)
                            .Include(c => c.Schedule)
                            .Where(c => listRequestedDate.Contains(c.Schedule.Date) &&
                            !requestedSchedule.CurrentStudySubjectId.Contains(c.StudySubject.Id)
                            && (c.TeacherId == requestedSchedule.TeacherId
                            || c.StudySubject.StudySubjectMember.Any(member => requestedSchedule.StudentIds.Contains(member.StudentId)))
                            && c.Status != ClassStatus.CANCELLED && c.Status != ClassStatus.DELETED)
                            .ToListAsync();

            var dbRequestedStudents = await _context.Students.Where(s => requestedSchedule.StudentIds.Contains(s.Id)).ToListAsync();

            var allCourse = await _context.Courses.Include(c => c.Subjects).Include(c => c.Levels).ToListAsync();

            var requestedStudySubject = dbAllStudySubjects.FirstOrDefault(s => s.Id == requestedSchedule.RequestedStudySubjectId);

            var requestedTeacher = await _context.Teachers.Include(x => x.Mandays)
                                                            .ThenInclude(x => x.WorkTimes)
                                                          .FirstOrDefaultAsync(t => t.Id == requestedSchedule.TeacherId)
                                                          ?? throw new NotFoundException($"Teacher with ID {requestedSchedule.TeacherId} is not found.");

            var dbAppointmentSchedules = await _context.Schedules
                                    .Include(s => s.AppointmentSlot)
                                        .ThenInclude(a => a!.Appointment)
                                            .ThenInclude(a => a.AppointmentMembers)
                                                .ThenInclude(m => m.Teacher)
                                    .Where(s => listRequestedDate.Contains(s.Date) && s.Type == ScheduleType.Appointment
                                    && s.AppointmentSlot!.Appointment.AppointmentMembers.Any(m => m.TeacherId == requestedTeacher.Id)
                                    && s.AppointmentSlot.AppointmentSlotStatus != AppointmentSlotStatus.DELETED).ToListAsync();

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
                        var conflict = conflictSchedule.FirstOrDefault(s => s.StudyCourseId == dbStudyClass.StudyCourseId);

                        if (conflict == null)
                        {
                            conflict = new ConflictScheduleResponseDto
                            {
                                Message = dbStudyClass.StudyCourse.StudyCourseType + " Course: " + dbStudyClass.StudyCourse.Course.course,
                                StudyCourseId = dbStudyClass.StudyCourseId,
                            };
                            conflictSchedule.Add(conflict);
                        }

                        if (!conflict.ConflictScheduleDetail.Any(s => s.ScheduleId == dbStudyClass.ScheduleId))
                        {
                            var conflictScheduleDetail = new ConflictScheduleDetailResponseDto
                            {
                                Message = dbStudyClass.Schedule.Date.ToDateString() + "("
                                                + dbStudyClass.Schedule.FromTime.ToTimeSpanString() + " - " + dbStudyClass.Schedule.ToTime.ToTimeSpanString() + "), "
                                                + dbStudyClass.StudyCourse.StudyCourseType + " Course: " + dbStudyClass.StudyCourse.Course.course + " "
                                                + dbStudyClass.StudySubject.Subject.subject + " " + (dbStudyClass.StudyCourse.Level?.level ?? ""),
                                ScheduleId = dbStudyClass.ScheduleId,
                            };

                            foreach (var dbStudent in dbRequestedStudents)
                            {
                                if (dbStudyClass.StudySubject.StudySubjectMember.Any(m => m.StudentId == dbStudent.Id))
                                {
                                    var conflictMember = new ConflictMemberResponseDto
                                    {
                                        Role = "Student",
                                        MemberId = dbStudent.Id,
                                        FirstName = dbStudent.FirstName,
                                        LastName = dbStudent.LastName,
                                        Nickname = dbStudent.Nickname,
                                        FullName = dbStudent.FullName,
                                    };
                                    conflictScheduleDetail.ConflictMembers.Add(conflictMember);
                                    if (!conflict.ConflictMembers.Any(m => m.MemberId == dbStudent.Id && m.Role == "Student"))
                                    {
                                        conflict.ConflictMembers.Add(conflictMember);
                                    }
                                }
                            }

                            if (dbStudyClass.TeacherId == requestedTeacher.Id)
                            {
                                var conflictMember = new ConflictMemberResponseDto
                                {
                                    Role = "Teacher",
                                    MemberId = requestedTeacher.Id,
                                    FirstName = requestedTeacher.FirstName,
                                    LastName = requestedTeacher.LastName,
                                    FullName = requestedTeacher.FullName,
                                    Nickname = requestedTeacher.Nickname,
                                };
                                conflictScheduleDetail.ConflictMembers.Add(conflictMember);
                                if (!conflict.ConflictMembers.Any(m => m.MemberId == requestedTeacher.Id && m.Role == "Teacher"))
                                {
                                    conflict.ConflictMembers.Add(conflictMember);
                                }
                            }
                            conflict.ConflictScheduleDetail.Add(conflictScheduleDetail);
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


                        var conflict = conflictSchedule.FirstOrDefault(s => s.Message == "Conflict with Current Course");
                        if (conflict == null)
                        {
                            conflict = new ConflictScheduleResponseDto
                            {
                                Message = "Conflict with Current Course",
                            };
                            conflictSchedule.Add(conflict);
                        }
                        var conflictScheduleDetail = new ConflictScheduleDetailResponseDto
                        {
                            Message = localSchedule.Date.ToDateTime().ToDateString() + "("
                                                + localSchedule.FromTime.ToTimeSpan().ToTimeSpanString()
                                                + " - " + localSchedule.ToTime.ToTimeSpan().ToTimeSpanString() + "), Conflict with Current Course: "
                                                + conflictCourse.course + " "
                                                + conflictSubject.subject + " " + (conflictLevel?.level ?? ""),
                        };

                        if (!conflict.ConflictScheduleDetail.Any(s => s.Message == conflictScheduleDetail.Message))
                        {
                            foreach (var dbStudent in dbRequestedStudents)
                            {
                                var conflictStudentMember = new ConflictMemberResponseDto
                                {
                                    Role = "Student",
                                    MemberId = dbStudent.Id,
                                    FirstName = dbStudent.FirstName,
                                    LastName = dbStudent.LastName,
                                    Nickname = dbStudent.Nickname,
                                    FullName = dbStudent.FullName,
                                };
                                conflictScheduleDetail.ConflictMembers.Add(conflictStudentMember);
                                if (!conflict.ConflictMembers.Any(m => m.MemberId == dbStudent.Id && m.Role == "Student"))
                                {
                                    conflict.ConflictMembers.Add(conflictStudentMember);
                                }
                            }

                            var conflictTeacherMember = new ConflictMemberResponseDto
                            {
                                Role = "Teacher",
                                MemberId = dbTeacher.Id,
                                FirstName = dbTeacher.FirstName,
                                LastName = dbTeacher.LastName,
                                FullName = dbTeacher.FullName,
                                Nickname = dbTeacher.Nickname,
                            };
                            conflictScheduleDetail.ConflictMembers.Add(conflictTeacherMember);
                            if (!conflict.ConflictMembers.Any(m => m.MemberId == requestedTeacher.Id && m.Role == "Teacher"))
                            {
                                conflict.ConflictMembers.Add(conflictTeacherMember);
                            }
                            conflict.ConflictScheduleDetail.Add(conflictScheduleDetail);
                        }
                    }
                }

                foreach (var dbAppointmentSchedule in dbAppointmentSchedules.Where(s => s.Date == requestDate))
                {
                    if (requestedSchedule.FromTime.ToTimeSpan().TotalMilliseconds < dbAppointmentSchedule.ToTime.TotalMilliseconds
                    && dbAppointmentSchedule.FromTime.TotalMilliseconds < requestedSchedule.ToTime.ToTimeSpan().TotalMilliseconds)
                    {
                        if (dbAppointmentSchedule.AppointmentSlot == null)
                            throw new InternalServerException("Something went wrong with Schedule Appointment");


                        var conflict = conflictSchedule.FirstOrDefault(c => c.AppointmentId == dbAppointmentSchedule.AppointmentSlot.AppointmentId);
                        if (conflict == null)
                        {
                            conflict = new ConflictScheduleResponseDto
                            {
                                Message = dbAppointmentSchedule.AppointmentSlot.Appointment.AppointmentType
                                            + " Appointment: " + dbAppointmentSchedule.AppointmentSlot.Appointment.Title,
                                AppointmentId = dbAppointmentSchedule.AppointmentSlot.AppointmentId,
                            };
                            conflictSchedule.Add(conflict);
                        }

                        var conflictScheduleDetail = new ConflictScheduleDetailResponseDto
                        {
                            ScheduleId = dbAppointmentSchedule.Id,
                            Message = dbAppointmentSchedule.Date.ToDateString() + "("
                                                + dbAppointmentSchedule.FromTime.ToTimeSpanString() + " - "
                                                + dbAppointmentSchedule.ToTime.ToTimeSpanString() + ")"
                        };

                        var conflictMember = new ConflictMemberResponseDto
                        {
                            Role = "Teacher",
                            MemberId = requestedTeacher.Id,
                            FirstName = requestedTeacher.FirstName,
                            LastName = requestedTeacher.LastName,
                            FullName = requestedTeacher.FullName,
                            Nickname = requestedTeacher.Nickname,
                        };

                        conflictScheduleDetail.ConflictMembers.Add(conflictMember);
                        if (!conflict.ConflictMembers.Any(m => m.MemberId == requestedTeacher.Id && m.Role == "Teacher"))
                        {
                            conflict.ConflictMembers.Add(conflictMember);
                        }
                        conflict.ConflictScheduleDetail.Add(conflictScheduleDetail);
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
                        TeacherShifts = _teacherService.GetTeacherWorkTypesWithHours(requestedTeacher, requestDate, requestedSchedule.FromTime.ToTimeSpan(), requestedSchedule.ToTime.ToTimeSpan())
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
                                        .ThenInclude(c => c!.StudyCourse)
                                            .ThenInclude(c => c.Course)
                                    .Include(s => s.StudyClass)
                                        .ThenInclude(c => c!.StudyCourse)
                                            .ThenInclude(c => c.Level)
                                    .Include(s => s.StudyClass)
                                        .ThenInclude(c => c!.StudySubject)
                                            .ThenInclude(c => c.Subject)
                                    .Where(s => requestedDate.Contains(s.Date) && s.Type == ScheduleType.Class
                                        && s.StudyClass != null
                                        && s.StudyClass.Status != ClassStatus.CANCELLED
                                        && s.StudyClass.Status != ClassStatus.DELETED).ToListAsync();

            var dbAppointmentSchedules = await _context.Schedules
                                    .Include(s => s.AppointmentSlot)
                                        .ThenInclude(a => a!.Appointment)
                                            .ThenInclude(a => a.AppointmentMembers)
                                                .ThenInclude(m => m.Teacher)
                                    .Where(s => requestedDate.Contains(s.Date) && s.Type == ScheduleType.Appointment
                                    && s.AppointmentSlot != null
                                    && s.AppointmentSlot.AppointmentId != appointmentId
                                    && s.AppointmentSlot.AppointmentSlotStatus != AppointmentSlotStatus.DELETED).ToListAsync();

            var dbTeachers = await _context.Teachers
                                .Include(t => t.Mandays)
                                    .ThenInclude(x => x.WorkTimes)
                                .Where(t => t.IsActive == true)
                                .ToListAsync();

            List<AvailableTeacherResponseDto> data = new();

            foreach (var dbTeacher in dbTeachers)
            {
                var conflictSchedules = new List<ConflictScheduleResponseDto>();
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
                            var conflictSchedule = conflictSchedules.FirstOrDefault(s => s.StudyCourseId == dbClassSchedule.StudyClass.StudyCourseId);

                            if (conflictSchedule == null)
                            {
                                conflictSchedule = new ConflictScheduleResponseDto
                                {
                                    Message = dbClassSchedule.StudyClass.StudyCourse.StudyCourseType + " Course: " + dbClassSchedule.StudyClass.StudyCourse.Course.course,
                                    StudyCourseId = dbClassSchedule.StudyClass.StudyCourseId,
                                };
                                conflictSchedules.Add(conflictSchedule);
                            }
                            conflictSchedule.ConflictScheduleDetail.Add(new ConflictScheduleDetailResponseDto
                            {
                                ScheduleId = dbClassSchedule.Id,
                                Message = dbClassSchedule.StudyClass.Schedule.Date.ToDateString() + "("
                                            + dbClassSchedule.StudyClass.Schedule.FromTime.ToTimeSpanString() + " - " + dbClassSchedule.StudyClass.Schedule.ToTime.ToTimeSpanString() + "), "
                                            + dbClassSchedule.StudyClass.StudyCourse.StudyCourseType + " Course: " + dbClassSchedule.StudyClass.StudyCourse.Course.course + " "
                                            + dbClassSchedule.StudyClass.StudySubject.Subject.subject + " " + (dbClassSchedule.StudyClass.StudyCourse.Level?.level ?? ""),
                            });
                        }
                    }

                    foreach (var dbAppointmentSchedule in dbAppointmentSchedules.Where(s => s.Date == appointmentRequest.Date.ToDateTime()))
                    {
                        if (appointmentRequest.FromTime.ToTimeSpan().TotalMilliseconds < dbAppointmentSchedule.ToTime.TotalMilliseconds
                        && dbAppointmentSchedule.FromTime.TotalMilliseconds < appointmentRequest.ToTime.ToTimeSpan().TotalMilliseconds
                        && dbAppointmentSchedule.AppointmentSlot?.Appointment?.AppointmentMembers?.Any(m => m.TeacherId == dbTeacher.Id) == true)
                        {
                            isConflict = true;
                            var conflictSchedule = conflictSchedules.FirstOrDefault(s => s.AppointmentId == dbAppointmentSchedule.AppointmentSlot.AppointmentId);
                            if (conflictSchedule == null)
                            {
                                conflictSchedule = new ConflictScheduleResponseDto
                                {
                                    Message = dbAppointmentSchedule.AppointmentSlot.Appointment.AppointmentType
                                            + " Appointment: " + dbAppointmentSchedule.AppointmentSlot.Appointment.Title,
                                    AppointmentId = dbAppointmentSchedule.AppointmentSlot.AppointmentId,
                                };
                                conflictSchedules.Add(conflictSchedule);

                                conflictSchedule.ConflictScheduleDetail.Add(new ConflictScheduleDetailResponseDto
                                {
                                    ScheduleId = dbAppointmentSchedule.Id,
                                    Message = dbAppointmentSchedule.Date.ToDateString() + "("
                                                + dbAppointmentSchedule.FromTime.ToTimeSpanString() + " - "
                                                + dbAppointmentSchedule.ToTime.ToTimeSpanString() + ")"
                                });

                            }
                        }
                    }
                }
                data.Add(new AvailableTeacherResponseDto
                {
                    IsConflict = isConflict,
                    TeacherId = dbTeacher.Id,
                    FirebaseId = dbTeacher.FirebaseId!,
                    FirstName = dbTeacher.FirstName,
                    LastName = dbTeacher.LastName,
                    FullName = dbTeacher.FullName,
                    Nickname = dbTeacher.Nickname,
                    ConflictSchedules = conflictSchedules,
                });
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
                                    .Where(s => requestDto.AppointmentSchedule.Select(a => a.Date.ToDateTime()).Contains(s.Date)
                                    && s.StudyClass != null
                                    && requestDto.TeacherIds.Contains(s.StudyClass.Teacher.Id)
                                    && s.StudyClass.Status != ClassStatus.DELETED && s.StudyClass.Status != ClassStatus.CANCELLED).ToListAsync();

            var dbAppointmentSchedules = await _context.Schedules
                                    .Include(s => s.AppointmentSlot)
                                        .ThenInclude(a => a!.Appointment)
                                            .ThenInclude(a => a.AppointmentMembers)
                                                .ThenInclude(m => m.Teacher)
                                    .Where(s => requestDto.AppointmentSchedule.Select(a => a.Date.ToDateTime()).Contains(s.Date)
                                    && s.AppointmentSlot != null
                                    && s.AppointmentSlot.AppointmentId != requestDto.AppointmentId
                                    && s.AppointmentSlot.AppointmentSlotStatus != AppointmentSlotStatus.DELETED
                                    && s.AppointmentSlot.Appointment.AppointmentMembers.Any(m => requestDto.TeacherIds.Contains(m.Teacher.Id)))
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
                            var conflictSchedule = data.ConflictSchedules.FirstOrDefault(s => s.StudyCourseId == dbSchedule.StudyClass.StudyCourseId);
                            if (conflictSchedule == null)
                            {
                                conflictSchedule = new ConflictScheduleResponseDto
                                {
                                    Message = dbSchedule.StudyClass.StudyCourse.StudyCourseType + " Course: " + dbSchedule.StudyClass.StudyCourse.Course.course,
                                    StudyCourseId = dbSchedule.StudyClass.StudyCourseId,
                                };
                                data.ConflictSchedules.Add(conflictSchedule);
                                data.IsConflict = true;
                            }
                            var conflictMember = new ConflictMemberResponseDto
                            {
                                Role = "Teacher",
                                MemberId = dbSchedule.StudyClass.Teacher.Id,
                                FirstName = dbSchedule.StudyClass.Teacher.FirstName,
                                LastName = dbSchedule.StudyClass.Teacher.LastName,
                                Nickname = dbSchedule.StudyClass.Teacher.Nickname,
                                FullName = dbSchedule.StudyClass.Teacher.FullName,
                            };

                            if (!conflictSchedule.ConflictMembers.Any(m => m.MemberId == dbSchedule.StudyClass.Teacher.Id))
                            {
                                conflictSchedule.ConflictMembers.Add(conflictMember);
                            };

                            var conflictScheduleDetail = new ConflictScheduleDetailResponseDto
                            {
                                ScheduleId = dbSchedule.Id,
                                Message = dbSchedule.Date.ToDateString() + "("
                                        + dbSchedule.FromTime.ToTimeSpanString() + " - " + dbSchedule.ToTime.ToTimeSpanString() + "), "
                                        + dbSchedule.StudyClass.StudyCourse.StudyCourseType + " Course: " + dbSchedule.StudyClass.StudyCourse.Course.course + " "
                                        + dbSchedule.StudyClass.StudySubject.Subject.subject + " " + (dbSchedule.StudyClass.StudyCourse.Level?.level ?? ""),
                            };
                            conflictScheduleDetail.ConflictMembers.Add(conflictMember);

                            if (!conflictSchedule.ConflictScheduleDetail.Any(sd => sd.Message == conflictScheduleDetail.Message))
                            {
                                conflictSchedule.ConflictScheduleDetail.Add(conflictScheduleDetail);
                            };
                        }
                    }
                }

                foreach (var dbSchedule in dbAppointmentSchedules.Where(s => s.Date == requestSchedule.Date.ToDateTime()))
                {
                    if (dbSchedule.FromTime < requestSchedule.ToTime.ToTimeSpan()
                        && requestSchedule.FromTime.ToTimeSpan() < dbSchedule.ToTime)
                    {
                        if (dbSchedule.AppointmentSlot != null)
                        {
                            var conflictSchedule = data.ConflictSchedules.FirstOrDefault(s => s.AppointmentId == dbSchedule.AppointmentSlot.AppointmentId);
                            if (conflictSchedule == null)
                            {
                                conflictSchedule = new ConflictScheduleResponseDto
                                {
                                    Message = dbSchedule.AppointmentSlot.Appointment.AppointmentType
                                            + " Appointment: " + dbSchedule.AppointmentSlot.Appointment.Title,
                                    AppointmentId = dbSchedule.AppointmentSlot.AppointmentId,
                                };
                                data.ConflictSchedules.Add(conflictSchedule);
                                data.IsConflict = true;
                            }
                            var conflictScheduleDetail = new ConflictScheduleDetailResponseDto
                            {
                                ScheduleId = dbSchedule.Id,
                                Message = dbSchedule.Date.ToDateString() + "("
                                            + dbSchedule.FromTime.ToTimeSpanString() + " - "
                                            + dbSchedule.ToTime.ToTimeSpanString() + ")"
                            };

                            var conflictTeachers = dbTeachers.Where(t => dbSchedule.AppointmentSlot.Appointment.AppointmentMembers.Select(m => m.TeacherId).Contains(t.Id)).ToList();

                            foreach (var conflictTeacher in conflictTeachers)
                            {
                                if (!conflictSchedule.ConflictMembers.Any(m => m.MemberId == conflictTeacher.Id))
                                {
                                    conflictSchedule.ConflictMembers.Add(new ConflictMemberResponseDto
                                    {
                                        Role = "Teacher",
                                        MemberId = conflictTeacher.Id,
                                        FirstName = conflictTeacher.FirstName,
                                        LastName = conflictTeacher.LastName,
                                        Nickname = conflictTeacher.Nickname,
                                        FullName = conflictTeacher.FullName,
                                    });
                                }
                                if (!conflictScheduleDetail.ConflictMembers.Any(m => m.MemberId == conflictTeacher.Id))
                                {
                                    conflictScheduleDetail.ConflictMembers.Add(new ConflictMemberResponseDto
                                    {
                                        Role = "Teacher",
                                        MemberId = conflictTeacher.Id,
                                        FirstName = conflictTeacher.FirstName,
                                        LastName = conflictTeacher.LastName,
                                        Nickname = conflictTeacher.Nickname,
                                        FullName = conflictTeacher.FullName,
                                    });
                                }
                            }
                            if (!conflictSchedule.ConflictScheduleDetail.Any(s => s.ScheduleId == dbSchedule.Id))
                            {
                                conflictSchedule.ConflictScheduleDetail.Add(conflictScheduleDetail);
                            }

                        }
                    }

                    foreach (var requestSchedule2 in requestDto.AppointmentSchedule.Where(s => s != requestSchedule && s.Date == requestSchedule.Date))
                    {
                        if (requestSchedule2.FromTime.ToTimeSpan() < requestSchedule.ToTime.ToTimeSpan()
                        && requestSchedule.FromTime.ToTimeSpan() < requestSchedule2.ToTime.ToTimeSpan())
                        {
                            var conflictSchedule = data.ConflictSchedules.FirstOrDefault(s => s.Message == "Current Appointment");
                            if (conflictSchedule == null)
                            {
                                conflictSchedule = new ConflictScheduleResponseDto
                                {
                                    Message = "Current Appointment",
                                };
                                data.ConflictSchedules.Add(conflictSchedule);
                                data.IsConflict = true;
                            }
                            var conflictScheduleDetail = new ConflictScheduleDetailResponseDto
                            {
                                Message = requestSchedule2.Date.ToDateTime().ToDateString() + "("
                                            + requestSchedule2.FromTime.ToTimeSpan().ToTimeSpanString() + " - "
                                            + requestSchedule2.ToTime.ToTimeSpan().ToTimeSpanString() + ")"
                            };

                            foreach (var dbTeacher in dbTeachers)
                            {
                                if (!conflictSchedule.ConflictMembers.Any(m => m.MemberId == dbTeacher.Id))
                                {
                                    conflictSchedule.ConflictMembers.Add(new ConflictMemberResponseDto
                                    {
                                        Role = "Teacher",
                                        MemberId = dbTeacher.Id,
                                        FirstName = dbTeacher.FirstName,
                                        LastName = dbTeacher.LastName,
                                        Nickname = dbTeacher.Nickname,
                                        FullName = dbTeacher.FullName,
                                    });
                                }
                                if (!conflictScheduleDetail.ConflictMembers.Any(m => m.MemberId == dbTeacher.Id))
                                    conflictScheduleDetail.ConflictMembers.Add(new ConflictMemberResponseDto
                                    {
                                        Role = "Teacher",
                                        MemberId = dbTeacher.Id,
                                        FirstName = dbTeacher.FirstName,
                                        LastName = dbTeacher.LastName,
                                        Nickname = dbTeacher.Nickname,
                                        FullName = dbTeacher.FullName,
                                    });
                            }
                            if (!conflictSchedule.ConflictScheduleDetail.Any(c => c.Message == conflictScheduleDetail.Message))
                            {
                                conflictSchedule.ConflictScheduleDetail.Add(conflictScheduleDetail);
                            }
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

        public async Task<ServiceResponse<StudentAddingConflictResponseDto>> CheckStudentAddingConflict(StudentAddingConflictRequestDto requestDto)
        {
            var requestedStudyClasses = await _context.StudyClasses
                                .Include(s => s.StudyCourse)
                                    .ThenInclude(c => c.Course)
                                .Include(s => s.StudySubject)
                                    .ThenInclude(c => c.Subject)
                                .Include(s => s.Schedule)
                                .Where(s => requestDto.StudySubjectIds.Contains(s.StudySubjectId))
                                .ToListAsync();

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
                            .Where(c => c.StudySubject.StudySubjectMember.Any(member => requestDto.StudentIds.Contains(member.StudentId))
                            && c.Status != ClassStatus.DELETED
                            && c.Status != ClassStatus.CANCELLED)
                            .ToListAsync();

            var dbStudents = await _context.Students.Where(s => requestDto.StudentIds.Contains(s.Id)).ToListAsync();

            StudentAddingConflictResponseDto data = new();
            foreach (var requestedStudyClass in requestedStudyClasses)
            {
                foreach (var dbStudyClass in dbStudyClasses.Where(c => c.Schedule.Date == requestedStudyClass.Schedule.Date))
                {
                    if (dbStudyClass.Schedule.FromTime < requestedStudyClass.Schedule.ToTime
                    && requestedStudyClass.Schedule.FromTime < dbStudyClass.Schedule.ToTime)
                    {
                        var conflict = new ConflictScheduleResponseDto
                        {
                            Message = dbStudyClass.Schedule.Date.ToDateString() + "("
                                            + dbStudyClass.Schedule.FromTime.ToTimeSpanString() + " - " + dbStudyClass.Schedule.ToTime.ToTimeSpanString() + "), "
                                            + dbStudyClass.StudyCourse.StudyCourseType + " Course: " + dbStudyClass.StudyCourse.Course.course + " "
                                            + dbStudyClass.StudySubject.Subject.subject + " " + (dbStudyClass.StudyCourse.Level?.level ?? ""),
                        };

                        foreach (var dbStudent in dbStudents)
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
                        if (!data.ConflictMessages.Any(c => c.Message == conflict.Message))
                        {
                            data.ConflictMessages.Add(conflict);
                            data.IsConflict = true;
                        }
                    }
                }

                foreach (var requestedStudyClass2 in requestedStudyClasses
                        .Where(c => c.Schedule.Date == requestedStudyClass.Schedule.Date))
                {
                    if (requestedStudyClass2.Schedule.FromTime < requestedStudyClass.Schedule.ToTime
                    && requestedStudyClass.Schedule.FromTime < requestedStudyClass2.Schedule.ToTime)
                    {
                        var conflict = new ConflictScheduleResponseDto
                        {
                            Message = requestedStudyClass2.Schedule.Date.ToDateString() + "("
                                            + requestedStudyClass2.Schedule.FromTime.ToTimeSpanString() + " - " + requestedStudyClass2.Schedule.ToTime.ToTimeSpanString() + "), "
                                            + requestedStudyClass2.StudyCourse.StudyCourseType + " Conflict with Current Course: " + requestedStudyClass2.StudyCourse.Course.course + " "
                                            + requestedStudyClass2.StudySubject.Subject.subject + " " + (requestedStudyClass2.StudyCourse.Level?.level ?? ""),
                        };

                        foreach (var dbStudent in dbStudents)
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

                        if (!data.ConflictMessages.Any(c => c.Message == conflict.Message))
                        {
                            data.ConflictMessages.Add(conflict);
                            data.IsConflict = true;
                        }

                    }
                }
            }

            return new ServiceResponse<StudentAddingConflictResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
        }
    }
}