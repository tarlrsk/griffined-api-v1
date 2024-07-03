using AutoMapper.Execution;
using Azure;
using Google.Api;
using griffined_api.Dtos.ScheduleDtos;
using griffined_api.Dtos.StudentReportDtos;
using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Extensions.DateTimeExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace griffined_api.Services.StudyCourseService
{
    public class StudyCourseService : IStudyCourseService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;
        private readonly ITeacherService _teacherService;

        public StudyCourseService(DataContext context, IFirebaseService firebaseService, ITeacherService teacherService)
        {
            _context = context;
            _firebaseService = firebaseService;
            _teacherService = teacherService;
        }
        public async Task<ServiceResponse<string>> AddGroupSchedule(GroupScheduleRequestDto newRequestedSchedule)
        {
            var response = new ServiceResponse<string>();

            var dbCourse = await _context.Courses
                            .Include(c => c.Subjects.Where(s => newRequestedSchedule.SubjectIds.Contains(s.Id)))
                            .FirstOrDefaultAsync(c => c.Id == newRequestedSchedule.CourseId)
                            ?? throw new NotFoundException($"Course is not found");

            Level? dbLevel = null;
            if (newRequestedSchedule.LevelId != null)
            {
                var foundLevel = await _context.Levels
                            .FirstOrDefaultAsync(l => l.Id == newRequestedSchedule.LevelId && l.CourseId == newRequestedSchedule.CourseId)
                            ?? throw new NotFoundException($"Level is not found in course {dbCourse.course}");

                dbLevel = foundLevel;
            }

            var studyCourse = new StudyCourse
            {
                Course = dbCourse,
                Level = dbLevel,
                Section = newRequestedSchedule.Section,
                TotalHour = newRequestedSchedule.TotalHours,
                StartDate = newRequestedSchedule.StartDate.ToDateTime(),
                EndDate = newRequestedSchedule.EndDate.ToDateTime(),
                StudyCourseType = StudyCourseType.Group,
                Method = newRequestedSchedule.Method,
                Status = StudyCourseStatus.NotStarted
            };

            var teachers = await _context.Teachers
                            .Include(t => t.Mandays)
                                .ThenInclude(x => x.WorkTimes)
                            .ToListAsync();

            foreach (var newStudySubject in dbCourse.Subjects)
            {
                var studySubject = new StudySubject();
                var classNumber = 1;
                double totalSubjectHour = 0;
                int studyClassCount = newRequestedSchedule.Schedules.Count();
                int c = 0;
                foreach (var newSchedule in newRequestedSchedule.Schedules)
                {
                    c++;

                    if (newSchedule.SubjectId == newStudySubject.Id)
                    {
                        var teacher = teachers.FirstOrDefault(t => t.Id == newSchedule.TeacherId) ?? throw new NotFoundException($"Teacher with ID {newSchedule.TeacherId} is not found.");

                        var studyClass = new StudyClass
                        {
                            IsMakeup = false,
                            IsFiftyPercent = false,
                            IsHundredPercent = false,
                            ClassNumber = classNumber,
                            Teacher = teacher,
                            StudyCourse = studyCourse,
                            Schedule = new Schedule
                            {
                                Date = newSchedule.Date.ToDateTime(),
                                FromTime = newSchedule.FromTime.ToTimeSpan(),
                                ToTime = newSchedule.ToTime.ToTimeSpan(),
                                Type = ScheduleType.Class,
                                CalendarType = DailyCalendarType.NORMAL_CLASS,
                            }
                        };

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

                        // var worktypes = _teacherService.GetTeacherWorkTypesWithHours(teacher, newSchedule.Date.ToDateTime(), newSchedule.FromTime.ToTimeSpan(), newSchedule.ToTime.ToTimeSpan());
                        // foreach (var worktype in worktypes)
                        // {
                        //     studyClass.TeacherShifts.Add(new TeacherShift
                        //     {
                        //         Teacher = teacher,
                        //         TeacherWorkType = worktype.TeacherWorkType,
                        //         Hours = worktype.Hours,
                        //     });
                        // }

                        classNumber = +1;
                        studySubject.StudyClasses.Add(studyClass);
                        totalSubjectHour += studyClass.Schedule.FromTime.Subtract(studyClass.Schedule.ToTime).TotalHours;
                    }
                }

                studySubject.Subject = newStudySubject;
                studySubject.Hour = totalSubjectHour;
                studyCourse.StudySubjects.Add(studySubject);
            }

            var studySubjects = studyCourse.StudySubjects.ToList();

            var teachersInCourse = studySubjects
                .SelectMany(ss => ss.StudyClasses)
                .Select(sc => sc.Teacher)
                .Distinct()
                .ToList();

            foreach (var teacher in teachersInCourse)
            {
                var teacherNotification = new TeacherNotification
                {
                    Teacher = teacher,
                    StudyCourse = studyCourse,
                    Title = "New Course Assigned",
                    Message = "You have been assigned to a new course. Click here for more details.",
                    DateCreated = DateTime.Now,
                    Type = TeacherNotificationType.NewCourse,
                    HasRead = false
                };

                _context.TeacherNotifications.Add(teacherNotification);
            }

            _context.StudyCourses.Add(studyCourse);

            await _context.SaveChangesAsync();
            response.StatusCode = (int)HttpStatusCode.OK; ;
            return response;
        }

        public async Task<ServiceResponse<List<StudyCourseResponseDto>>> GetAllStudyCourse()
        {
            var response = new ServiceResponse<List<StudyCourseResponseDto>>();
            var dbStudyCourses = await _context.StudyCourses
                                        .Include(c => c.StudySubjects)
                                            .ThenInclude(s => s.StudyClasses)
                                                .ThenInclude(c => c.Schedule)
                                        .Include(c => c.StudySubjects)
                                            .ThenInclude(s => s.StudyClasses)
                                                .ThenInclude(c => c.Teacher)
                                        .Include(c => c.StudySubjects)
                                            .ThenInclude(s => s.StudyClasses)
                                                .ThenInclude(c => c.TeacherShifts)
                                        .Include(c => c.StudySubjects)
                                            .ThenInclude(s => s.StudySubjectMember)
                                                .ThenInclude(s => s.Student)
                                        .Include(c => c.StudySubjects)
                                            .ThenInclude(s => s.Subject)
                                        .Include(c => c.Course)
                                        .Include(c => c.Level)
                                        .ToListAsync();

            var studyCourses = new List<StudyCourseResponseDto>();
            foreach (var dbStudyCourse in dbStudyCourses)
            {
                var studyCourse = new StudyCourseResponseDto
                {
                    StudyCourseId = dbStudyCourse.Id,
                    Section = dbStudyCourse.Section,
                    Course = dbStudyCourse.Course.course,
                    Level = dbStudyCourse.Level?.level,
                    TotalHour = dbStudyCourse.TotalHour,
                    StartDate = dbStudyCourse.StartDate.ToDateString(),
                    EndDate = dbStudyCourse.EndDate.ToDateString(),
                    Method = dbStudyCourse.Method,
                    StudyCourseType = dbStudyCourse.StudyCourseType,
                    CourseStatus = dbStudyCourse.Status
                };

                var studentCount = 0;
                var student = new List<int>();
                foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
                {
                    foreach (var dbMember in dbStudySubject.StudySubjectMember)
                    {
                        if (!student.Exists(s => s == dbMember.StudentId))
                        {
                            studentCount += 1;
                            student.Add(dbMember.StudentId);
                            studyCourse.Members.Add(new StudentNameResponseDto
                            {
                                StudentId = dbMember.Student.Id,
                                StudentCode = dbMember.Student.StudentCode!,
                                FirstName = dbMember.Student.FirstName,
                                LastName = dbMember.Student.LastName,
                                FullName = dbMember.Student.FullName,
                                Nickname = dbMember.Student.Nickname,
                            });
                        }
                    }
                    studyCourse.StudySubjects.Add(new StudySubjectResponseDto()
                    {
                        StudySubjectId = dbStudySubject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject,
                        Hour = dbStudySubject.Hour,
                    });
                    foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                    {
                        var schedule = new ScheduleResponseDto()
                        {
                            StudyCourseId = dbStudyCourse.Id,
                            Day = dbStudyClass.Schedule.Date.DayOfWeek.ToString().ToUpper(),
                            CourseId = dbStudyCourse.Course.Id,
                            CourseName = dbStudyCourse.Course.course,
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            SubjectName = dbStudySubject.Subject.subject,
                            CourseSubject = dbStudyCourse.Course.course + " " + dbStudySubject.Subject.subject + " " + (dbStudyCourse.Level?.level ?? ""),
                            StudyClassId = dbStudyClass.Id,
                            ClassNo = dbStudyClass.ClassNumber,
                            Room = null,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                            ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
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
                        studyCourse.Schedules.Add(schedule);
                    }
                }

                studyCourse.StudentCount = studentCount;
                studyCourses.Add(studyCourse);
            }

            response.Data = studyCourses;
            response.StatusCode = (int)HttpStatusCode.OK; ;
            return response;
        }

        public async Task<ServiceResponse<string>> AddNewStudyClass(List<NewStudyClassScheduleRequestDto> newStudyClasses, int requestId)
        {
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
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEA) ?? throw new NotFoundException($"Pending EA Request with ID {requestId} is not found.");

            var dbTeachers = await _context.Teachers
                            .Include(t => t.Mandays)
                                .ThenInclude(x => x.WorkTimes)
                            .ToListAsync();

            foreach (var dbNewRequestedCourse in dbRequest.NewCourseRequests)
            {
                var studyCourse = new StudyCourse()
                {
                    Course = dbNewRequestedCourse.Course,
                    Level = dbNewRequestedCourse.Level,
                    Section = dbRequest.Section,
                    TotalHour = dbNewRequestedCourse.TotalHours,
                    StartDate = dbNewRequestedCourse.StartDate,
                    EndDate = dbNewRequestedCourse.EndDate,
                    StudyCourseType = dbNewRequestedCourse.StudyCourseType,
                    Method = dbNewRequestedCourse.Method,
                    Status = StudyCourseStatus.Pending,
                    NewCourseRequest = dbNewRequestedCourse,
                };
                foreach (var dbNewRequestedSubject in dbNewRequestedCourse.NewCourseSubjectRequests)
                {
                    var studySubject = new StudySubject()
                    {
                        Subject = dbNewRequestedSubject.Subject,
                        Hour = dbNewRequestedSubject.Hour,
                    };
                    foreach (var student in dbRequest.RegistrationRequestMembers)
                    {
                        var member = new StudySubjectMember()
                        {
                            Student = student.Student,
                            CourseJoinedDate = DateTime.Now,
                            Status = StudySubjectMemberStatus.Pending,
                        };
                        studySubject.StudySubjectMember.Add(member);
                    }
                    var requestedStudyClasses = newStudyClasses.Where(c => c.SubjectId == dbNewRequestedSubject.SubjectId && c.CourseId == dbNewRequestedCourse.CourseId);

                    int studyClassCount = requestedStudyClasses.Count();
                    int c = 0;

                    foreach (var requestedStudyClass in requestedStudyClasses)
                    {
                        c++;

                        var dbTeacher = dbTeachers.FirstOrDefault(t => t.Id == requestedStudyClass.TeacherId) ?? throw new NotFoundException($"Teacher with ID {requestedStudyClass.TeacherId} is not found.");

                        var studyClass = new StudyClass
                        {
                            IsMakeup = false,
                            IsFiftyPercent = false,
                            IsHundredPercent = false,
                            ClassNumber = requestedStudyClass.ClassNo,
                            Teacher = dbTeacher,
                            StudyCourse = studyCourse,
                            Schedule = new Schedule()
                            {
                                Date = requestedStudyClass.Date.ToDateTime(),
                                FromTime = requestedStudyClass.FromTime.ToTimeSpan(),
                                ToTime = requestedStudyClass.ToTime.ToTimeSpan(),
                                Type = ScheduleType.Class,
                                CalendarType = DailyCalendarType.NORMAL_CLASS,
                            }
                        };

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

                        // var worktypes = _teacherService.GetTeacherWorkTypesWithHours(
                        //                 dbTeacher,
                        //                 requestedStudyClass.Date.ToDateTime(),
                        //                 requestedStudyClass.FromTime.ToTimeSpan(),
                        //                 requestedStudyClass.ToTime.ToTimeSpan());

                        // foreach (var worktype in worktypes)
                        // {
                        //     studyClass.TeacherShifts.Add(new TeacherShift
                        //     {
                        //         Teacher = dbTeacher,
                        //         TeacherWorkType = worktype.TeacherWorkType,
                        //         Hours = worktype.Hours,
                        //     });
                        // }

                        studySubject.StudyClasses.Add(studyClass);
                    }
                    studyCourse.StudySubjects.Add(studySubject);
                }
                _context.StudyCourses.Add(studyCourse);
            }

            dbRequest.RegistrationStatus = RegistrationStatus.PendingEC;
            dbRequest.ScheduledByStaffId = _firebaseService.GetAzureIdWithToken();
            dbRequest.HasSchedule = true;

            var ec = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Id == dbRequest.CreatedByStaffId)
                    ?? throw new NotFoundException("EC not found.");

            var ea = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Id == dbRequest.ScheduledByStaffId)
                    ?? throw new NotFoundException("EA not found.");

            var ecNotification = new StaffNotification
            {
                Staff = ec,
                RegistrationRequest = dbRequest,
                Title = "New Schedule Created",
                Message = $"A new schedule for Section '{dbRequest.Section}' has been created by EA {ea.Nickname}. Click here for more details.",
                DateCreated = DateTime.Now,
                Type = StaffNotificationType.RegistrationRequest,
                HasRead = false
            };

            _context.StaffNotifications.Add(ecNotification);

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<String> { StatusCode = (int)HttpStatusCode.OK };

            return response;
        }

        public async Task<ServiceResponse<string>> EditStudyClassByRegisRequest(EditStudyClassByRegistrationRequestDto requestDto, int requestId)
        {
            if (requestDto.ClassToDelete.Count != 0)
            {
                var dbStudyClasses = await _context.StudyClasses.Where(s => requestDto.ClassToDelete.Contains(s.Id)).ToListAsync();
                foreach (var dbStudyClass in dbStudyClasses)
                {
                    dbStudyClass.Status = ClassStatus.DELETED;
                }
            }

            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.Course)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEA) ?? throw new NotFoundException($"Pending EA Request with ID {requestId} is not found.");

            if (requestDto.ClassToAdd.Count != 0)
            {
                var dbTeachers = await _context.Teachers.ToListAsync();

                foreach (var dbNewCourseRequest in dbRequest.NewCourseRequests)
                {
                    if (dbNewCourseRequest.StudyCourse == null)
                        throw new InternalServerException("Something went wrong with NewCourseRequest and StudyCourse");


                    foreach (var dbStudySubject in dbNewCourseRequest.StudyCourse.StudySubjects)
                    {
                        var newStudyClasses = requestDto.ClassToAdd
                                            .Where(c => c.CourseId == dbNewCourseRequest.CourseId
                                            && c.SubjectId == dbStudySubject.SubjectId);

                        int studyClassCount = newStudyClasses.Count();
                        int c = 0;

                        foreach (var newStudyClass in newStudyClasses)
                        {
                            var studyClass = new StudyClass
                            {
                                IsFiftyPercent = false,
                                IsHundredPercent = false,
                                ClassNumber = newStudyClass.ClassNo,
                                Teacher = dbTeachers.FirstOrDefault(t => t.Id == newStudyClass.TeacherId) ?? throw new Exception($"Cannot Find Teacher ID {newStudyClass.TeacherId}"),
                                StudyCourse = dbNewCourseRequest.StudyCourse,
                                Schedule = new Schedule
                                {
                                    Date = newStudyClass.Date.ToDateTime(),
                                    FromTime = newStudyClass.FromTime.ToTimeSpan(),
                                    ToTime = newStudyClass.ToTime.ToTimeSpan(),
                                    Type = ScheduleType.Class,
                                    CalendarType = DailyCalendarType.NORMAL_CLASS,
                                }
                            };

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

                            dbStudySubject.StudyClasses.Add(studyClass);
                        }
                    }
                }
            }

            dbRequest.RegistrationStatus = RegistrationStatus.PendingEC;
            dbRequest.ScheduledByStaffId = _firebaseService.GetAzureIdWithToken();
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<List<StudyCourseMobileResponseDto>>> ListAllStudyCourseByStudentToken()
        {
            var studentId = _firebaseService.GetAzureIdWithToken();
            var dbStudyCourses = await _context.StudySubjectMember
                                .Include(m => m.StudySubject)
                                    .ThenInclude(s => s.StudyCourse)
                                        .ThenInclude(s => s.Course)
                                .Include(m => m.StudySubject)
                                    .ThenInclude(s => s.StudyCourse)
                                        .ThenInclude(s => s.Level)
                                .Include(m => m.StudySubject)
                                    .ThenInclude(s => s.Subject)
                                .Where(s => s.StudentId == studentId)
                                .GroupBy(m => m.StudySubject.StudyCourse)
                                .Select(group => new
                                {
                                    StudyCourse = group.Key,
                                    StudySubjects = group.Select(m => m.StudySubject)
                                })
                                .ToListAsync();

            var responseData = new List<StudyCourseMobileResponseDto>();
            foreach (var dbStudyCourse in dbStudyCourses)
            {
                var studyCourse = new StudyCourseMobileResponseDto
                {
                    Section = dbStudyCourse.StudyCourse.Section,
                    StudyCourseId = dbStudyCourse.StudyCourse.Id,
                    Course = dbStudyCourse.StudyCourse.Course.course,
                    Level = dbStudyCourse.StudyCourse.Level?.level,
                    LevelId = dbStudyCourse.StudyCourse.Level?.Id,
                    StudyCourseType = dbStudyCourse.StudyCourse.StudyCourseType,
                };
                foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
                {
                    var studySubject = new StudySubjectResponseDto
                    {
                        StudySubjectId = dbStudySubject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject,
                        Hour = dbStudySubject.Hour,
                    };
                    studyCourse.StudySubjects.Add(studySubject);
                }
                responseData.Add(studyCourse);
            }

            var response = new ServiceResponse<List<StudyCourseMobileResponseDto>>()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = responseData,
            };
            return response;
        }

        public async Task<ServiceResponse<List<StudyCourseMobileResponseDto>>> ListAllStudyCourseByTeacherToken()
        {
            var teacherId = _firebaseService.GetAzureIdWithToken();
            var dbStudyCourses = await _context.StudyClasses
                                .Include(c => c.StudySubject)
                                    .ThenInclude(s => s.StudyCourse)
                                        .ThenInclude(s => s.Course)
                                .Include(m => m.StudySubject)
                                    .ThenInclude(s => s.StudyCourse)
                                        .ThenInclude(s => s.Level)
                                .Include(m => m.StudyCourse)
                                    .ThenInclude(s => s.Course)
                                .Include(m => m.StudySubject)
                                    .ThenInclude(s => s.Subject)
                                .Where(s => s.TeacherId == teacherId && s.StudyCourse.Status != StudyCourseStatus.Pending
                                && s.StudyCourse.Status != StudyCourseStatus.Cancelled)
                                .GroupBy(m => m.StudySubject.StudyCourse)
                                .Select(group => new
                                {
                                    StudyCourse = group.Key,
                                    StudySubjects = group.Select(m => m.StudySubject)
                                })
                                .ToListAsync();

            var responseData = new List<StudyCourseMobileResponseDto>();
            foreach (var dbStudyCourse in dbStudyCourses)
            {
                var studyCourse = new StudyCourseMobileResponseDto
                {
                    Section = dbStudyCourse.StudyCourse.Section,
                    StudyCourseId = dbStudyCourse.StudyCourse.Id,
                    Course = dbStudyCourse.StudyCourse.Course.course,
                    Level = dbStudyCourse.StudyCourse.Level?.level,
                    LevelId = dbStudyCourse.StudyCourse.Level?.Id,
                    StudyCourseType = dbStudyCourse.StudyCourse.StudyCourseType,
                };
                foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
                {
                    var studySubject = new StudySubjectResponseDto
                    {
                        StudySubjectId = dbStudySubject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject,
                        Hour = dbStudySubject.Hour,
                    };
                    studyCourse.StudySubjects.Add(studySubject);
                }
                responseData.Add(studyCourse);
            }

            var response = new ServiceResponse<List<StudyCourseMobileResponseDto>>()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = responseData,
            };
            return response;
        }
        public async Task<ServiceResponse<StudyCourseMobileTeacherDetailResponseDto>> StudyCourseDetailForTeacher(int studyCourseId)
        {
            var dbStudyCourse = await _context.StudyCourses
                                .Include(c => c.Course)
                                .Include(c => c.Level)
                                .Include(c => c.StudySubjects)
                                    .ThenInclude(c => c.Subject)
                                .Include(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudyClasses)
                                        .ThenInclude(c => c.Schedule)
                                .Include(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudyClasses)
                                        .ThenInclude(c => c.Teacher)
                                            .ThenInclude(t => t.Mandays)
                                                .ThenInclude(x => x.WorkTimes)
                                .FirstOrDefaultAsync(c => c.Id == studyCourseId)
                                ?? throw new NotFoundException($"Study Course with ID {studyCourseId} is not found.");

            var studentCount = 0;
            var student = new List<int>();
            foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
            {
                foreach (var dbMember in dbStudySubject.StudySubjectMember)
                {
                    if (!student.Exists(s => s == dbMember.StudentId))
                    {
                        studentCount += 1;
                        student.Add(dbMember.StudentId);
                    }
                }
            }

            var data = new StudyCourseMobileTeacherDetailResponseDto()
            {
                StudyCourseId = dbStudyCourse.Id,
                StudyCourseType = dbStudyCourse.StudyCourseType,
                Section = dbStudyCourse.Section,
                Course = dbStudyCourse.Course.course,
                Level = dbStudyCourse.Level?.level,
                StudentCount = studentCount,
                TotalHour = dbStudyCourse.TotalHour,
                StartDate = dbStudyCourse.StartDate.ToDateString(),
                EndDate = dbStudyCourse.EndDate.ToDateString(),
                Method = dbStudyCourse.Method,
                CourseStatus = dbStudyCourse.Status,
            };

            var schedules = new List<ScheduleResponseDto>();
            foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
            {
                data.StudySubjects.Add(new StudySubjectResponseDto
                {
                    StudySubjectId = dbStudySubject.Id,
                    SubjectId = dbStudySubject.Subject.Id,
                    Subject = dbStudySubject.Subject.subject,
                    Hour = dbStudySubject.Hour,
                });
                foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                {
                    if (dbStudyClass.Status != ClassStatus.DELETED)
                    {
                        schedules.Add(new ScheduleResponseDto
                        {
                            Day = dbStudyClass.Schedule.Date.DayOfWeek.ToString().ToUpper(),
                            StudyClassId = dbStudyClass.Id,
                            ClassNo = dbStudyClass.ClassNumber,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                            ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                            StudyCourseId = dbStudyCourse.Id,
                            CourseId = dbStudyCourse.Course.Id,
                            CourseName = dbStudyCourse.Course.course,
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            SubjectName = dbStudySubject.Subject.subject,
                            CourseSubject = dbStudyCourse.Course.course + " "
                                            + dbStudySubject.Subject.subject
                                            + " " + (dbStudyCourse.Level?.level ?? ""),
                            ClassStatus = dbStudyClass.Status,
                            Teacher = new TeacherNameResponseDto
                            {
                                TeacherId = dbStudyClass.Teacher.Id,
                                FirstName = dbStudyClass.Teacher.FirstName,
                                LastName = dbStudyClass.Teacher.LastName,
                                Nickname = dbStudyClass.Teacher.Nickname,
                                FullName = dbStudyClass.Teacher.FullName,
                            },
                            IsFiftyPercent = dbStudyClass.IsFiftyPercent,
                            IsHundredPercent = dbStudyClass.IsHundredPercent
                        });
                    }

                }
            }

            data.Schedules = schedules.OrderBy(s => (s.Date + " " + s.FromTime).ToDateTime()).ToList();

            var response = new ServiceResponse<StudyCourseMobileTeacherDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
            return response;
        }

        public async Task<ServiceResponse<StudyCourseMobileStudentDetailResponseDto>> StudyCourseDetailForStudent(int studyCourseId)
        {
            var studentId = _firebaseService.GetAzureIdWithToken();
            var dbStudySubjects = await _context.StudySubjectMember
                                .Where(m => m.StudentId == studentId)
                                .Include(s => s.StudySubject.Subject)
                                .Where(s => s.StudySubject.StudyCourseId == studyCourseId)
                                .Select(m => m.StudySubject)
                                .Distinct()
                                .ToListAsync();

            var dbStudyCourse = await _context.StudyCourses
                                .Include(c => c.Course)
                                .Include(c => c.Level)
                                .FirstOrDefaultAsync(c => c.Id == studyCourseId)
                                ?? throw new NotFoundException($"Study Course with ID {studyCourseId} is not found.");

            var dbStudyClasses = await _context.StudyClasses
                                .Include(c => c.Schedule)
                                .Include(c => c.Attendances)
                                    .ThenInclude(a => a.Student)
                                .Include(c => c.Teacher)
                                .Where(c => dbStudySubjects.Contains(c.StudySubject) && c.Status != ClassStatus.DELETED)
                                .Select(c => new
                                {
                                    StudyClass = c,
                                    c.StudySubject,
                                    c.Schedule,
                                    Attendance = c.Attendances.FirstOrDefault(a => a.StudentId == studentId),
                                    c.IsFiftyPercent,
                                    c.IsHundredPercent
                                })
                                .ToListAsync();

            if (dbStudyClasses.Count == 0)
                throw new NotFoundException($"Class is not found");

            var data = new StudyCourseMobileStudentDetailResponseDto()
            {
                StudyCourseId = dbStudyCourse.Id,
                StudyCourseType = dbStudyCourse.StudyCourseType,
                Section = dbStudyCourse.Section,
                Course = dbStudyCourse.Course.course,
                Level = dbStudyCourse.Level?.level,
                TotalHour = dbStudyCourse.TotalHour,
                StartDate = dbStudyCourse.StartDate.ToDateString(),
                EndDate = dbStudyCourse.EndDate.ToDateString(),
                Method = dbStudyCourse.Method,
                CourseStatus = dbStudyCourse.Status,
            };

            var schedules = new List<ScheduleStudentMobileResponseDto>();
            foreach (var dbStudySubject in dbStudySubjects)
            {
                data.StudySubjects.Add(new StudySubjectResponseDto
                {
                    StudySubjectId = dbStudySubject.Id,
                    SubjectId = dbStudySubject.Subject.Id,
                    Subject = dbStudySubject.Subject.subject,
                    Hour = dbStudySubject.Hour,
                });
            }

            foreach (var dbStudyClass in dbStudyClasses)
            {
                if (dbStudyClass.Attendance == null)
                    throw new InternalServerException("Something went wrong with Student Attendance");
                schedules.Add(new ScheduleStudentMobileResponseDto
                {
                    StudyClassId = dbStudyClass.StudyClass.Id,
                    ClassNo = dbStudyClass.StudyClass.ClassNumber,
                    Date = dbStudyClass.StudyClass.Schedule.Date.ToDateString(),
                    FromTime = dbStudyClass.StudyClass.Schedule.FromTime.ToTimeSpanString(),
                    ClassStatus = dbStudyClass.StudyClass.Status,
                    ToTime = dbStudyClass.StudyClass.Schedule.ToTime.ToTimeSpanString(),
                    StudyCourseId = dbStudyCourse.Id,
                    CourseId = dbStudyCourse.Course.Id,
                    Course = dbStudyCourse.Course.course,
                    SubjectId = dbStudyClass.StudySubject.Subject.Id,
                    Subject = dbStudyClass.StudySubject.Subject.subject,
                    CourseSubject = dbStudyCourse.Course.course + " "
                                        + dbStudyClass.StudySubject.Subject.subject
                                        + " " + (dbStudyCourse.Level?.level ?? ""),
                    TeacherId = dbStudyClass.StudyClass.Teacher.Id,
                    TeacherFirstName = dbStudyClass.StudyClass.Teacher.FirstName,
                    TeacherLastName = dbStudyClass.StudyClass.Teacher.LastName,
                    TeacherNickname = dbStudyClass.StudyClass.Teacher.Nickname,
                    Attendance = dbStudyClass.Attendance.Attendance,
                    IsFiftyPercent = dbStudyClass.IsFiftyPercent,
                    IsHundredPercent = dbStudyClass.IsHundredPercent
                });
            }

            data.Schedules = schedules.OrderBy(s => (s.Date + " " + s.FromTime).ToDateTime()).ToList();

            var response = new ServiceResponse<StudyCourseMobileStudentDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateStudyClassRoom(int studyClassId, string room)
        {
            var dbClass = await _context.StudyClasses.Include(c => c.Schedule).FirstOrDefaultAsync(c => c.Id == studyClassId) ?? throw new NotFoundException($"Class with ID {studyClassId} not found.");

            dbClass.Schedule.Room = room;

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };

            return response;
        }

        public async Task<ServiceResponse<List<StudyCourseByStudentIdResponseDto>>> ListAllStudyCoursesWithReportsByStudentId(string studentCode)
        {
            var response = new ServiceResponse<List<StudyCourseByStudentIdResponseDto>>()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = new List<StudyCourseByStudentIdResponseDto>()
            };

            var dbStudyCourses = await _context.StudyCourses
                                .Include(sc => sc.Course)
                                .Include(sc => sc.Level)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.Subject)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudySubjectMember)
                                        .ThenInclude(sm => sm.Student)
                                .Include(sm => sm.StudySubjects)
                                    .ThenInclude(ss => ss.StudySubjectMember)
                                        .ThenInclude(sm => sm.StudentReports)
                                            .ThenInclude(sr => sr.Teacher)
                                .Where(sc => sc.StudySubjects.Any(ss => ss.StudySubjectMember.Any(sm => sm.Student.StudentCode == studentCode)))
                                .ToListAsync() ?? throw new NotFoundException($"No courses containing student with code {studentCode} found.");

            foreach (var dbStudyCourse in dbStudyCourses)
            {
                var studyCourseDto = new StudyCourseByStudentIdResponseDto
                {
                    StudentId = dbStudyCourse.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.Id)
                            .FirstOrDefault()!,
                    StudentCode = dbStudyCourse.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.StudentCode)
                            .FirstOrDefault()!,
                    StudentFirstName = dbStudyCourse.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.FirstName)
                            .FirstOrDefault()!,
                    StudentLastName = dbStudyCourse.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.LastName)
                            .FirstOrDefault()!,
                    StudentNickname = dbStudyCourse.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.Nickname)
                            .FirstOrDefault()!,
                    StudyCourseId = dbStudyCourse.Id,
                    Course = dbStudyCourse.Course.course,
                    Level = dbStudyCourse.Level!.level,
                    Section = dbStudyCourse.Section,
                    Status = dbStudyCourse.Status,
                    Reports = new List<StudySubjectReportResponseDto>()
                };

                foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
                {
                    var studentReport = dbStudySubject.StudySubjectMember
                                .FirstOrDefault(sm => sm.Student.StudentCode == studentCode)?
                                .StudentReports?
                                .FirstOrDefault();

                    var reportDto = new StudySubjectReportResponseDto
                    {
                        StudySubject = new StudySubjectResponseDto
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            Hour = dbStudySubject.Hour,
                        },
                        FiftyPercentReport = studentReport?.Progression == Progression.FiftyPercent
                            ? new ReportFileResponseDto
                            {
                                UploadedBy = studentReport.Teacher.Id,
                                Progression = studentReport.Progression,
                                File = new FilesResponseDto
                                {
                                    FileName = studentReport.FileName,
                                    ContentType = await _firebaseService.GetContentTypeByObjectName(studentReport.ObjectName),
                                    URL = await _firebaseService.GetUrlByObjectName(studentReport.ObjectName)
                                }
                            }
                            : null,
                        HundredPercentReport = studentReport?.Progression == Progression.HundredPercent
                            ? new ReportFileResponseDto
                            {
                                UploadedBy = studentReport.Teacher.Id,
                                Progression = studentReport.Progression,
                                File = new FilesResponseDto
                                {
                                    FileName = studentReport.FileName,
                                    ContentType = await _firebaseService.GetContentTypeByObjectName(studentReport.ObjectName),
                                    URL = await _firebaseService.GetUrlByObjectName(studentReport.ObjectName)
                                }
                            }
                            : null,
                        SpecialReport = studentReport?.Progression == Progression.Special
                            ? new ReportFileResponseDto
                            {
                                UploadedBy = studentReport.Teacher.Id,
                                Progression = studentReport.Progression,
                                File = new FilesResponseDto
                                {
                                    FileName = studentReport.FileName,
                                    ContentType = await _firebaseService.GetContentTypeByObjectName(studentReport.ObjectName),
                                    URL = await _firebaseService.GetUrlByObjectName(studentReport.ObjectName)
                                }
                            }
                            : null
                    };

                    studyCourseDto.Reports.Add(reportDto);
                }

                response.Data.Add(studyCourseDto);
            }

            return response;
        }

        public async Task<ServiceResponse<List<StudyCourseByTeacherIdResponseDto>>> ListAllStudyCoursesWithReportsByTeacherId(int teacherId)
        {
            var response = new ServiceResponse<List<StudyCourseByTeacherIdResponseDto>>()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = new List<StudyCourseByTeacherIdResponseDto>()
            };

            var dbStudyCourses = await _context.StudyCourses
                                .Include(sc => sc.Course)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.Subject)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudyClasses)
                                        .ThenInclude(sc => sc.Teacher)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudySubjectMember)
                                        .ThenInclude(sm => sm.Student)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudySubjectMember)
                                        .ThenInclude(sm => sm.StudentReports)
                                .Where(sc => sc.StudySubjects.Any(ss => ss.StudyClasses.Any(sm => sm.Teacher.Id == teacherId)))
                                .ToListAsync() ?? throw new NotFoundException("No courses containing teacher with ID {teacherId} found.");


            foreach (var dbStudyCourse in dbStudyCourses)
            {
                var studentCount = 0;
                var student = new List<int>();
                foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
                {
                    foreach (var dbMember in dbStudySubject.StudySubjectMember)
                    {
                        if (!student.Exists(s => s == dbMember.StudentId))
                        {
                            studentCount += 1;
                            student.Add(dbMember.StudentId);
                        }
                    }
                }

                var studyCourseDto = new StudyCourseByTeacherIdResponseDto
                {
                    TeacherId = dbStudyCourse.StudySubjects.FirstOrDefault()!.StudyClasses.FirstOrDefault()!.Teacher.Id,
                    TeacherFirstName = dbStudyCourse.StudySubjects.FirstOrDefault()!.StudyClasses.FirstOrDefault()!.Teacher.FirstName,
                    TeacherLastName = dbStudyCourse.StudySubjects.FirstOrDefault()!.StudyClasses.FirstOrDefault()!.Teacher.LastName,
                    TeacherNickname = dbStudyCourse.StudySubjects.FirstOrDefault()!.StudyClasses.FirstOrDefault()!.Teacher.Nickname,

                    StudyCourseId = dbStudyCourse.Id,
                    Course = dbStudyCourse.Course.course,
                    Level = dbStudyCourse.Level!.level,
                    Section = dbStudyCourse.Section,
                    StudentCount = studentCount,
                    TotalHour = dbStudyCourse.TotalHour,
                    StartDate = dbStudyCourse.StartDate.ToDateString(),
                    EndDate = dbStudyCourse.EndDate.ToDateString(),
                    Method = dbStudyCourse.Method
                };

                foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
                {
                    var studySubjectDto = new StudySubjectWithMembersResponseDto
                    {
                        StudySubjectId = dbStudySubject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject
                    };

                    foreach (var dbStudySubjectMember in dbStudySubject.StudySubjectMember)
                    {
                        var studentReport = dbStudySubject.StudySubjectMember
                                            .FirstOrDefault(sm => sm.Student.StudentCode == dbStudySubjectMember.Student.StudentCode)?
                                            .StudentReports?
                                            .FirstOrDefault();

                        var reportDto = new StudySubjectMemberWithReportsResponseDto
                        {
                            StudentId = dbStudySubjectMember.Student.Id,
                            StudentCode = dbStudySubjectMember.Student.StudentCode!,
                            StudentFirstName = dbStudySubjectMember.Student.FirstName,
                            StudentLastName = dbStudySubjectMember.Student.LastName,
                            StudentNickname = dbStudySubjectMember.Student.Nickname,
                            FiftyPercentReport = studentReport?.Progression == Progression.FiftyPercent
                                ? new ReportFileResponseDto
                                {
                                    UploadedBy = studentReport.Teacher.Id,
                                    Progression = studentReport.Progression,
                                    File = new FilesResponseDto
                                    {
                                        FileName = studentReport.FileName,
                                        ContentType = await _firebaseService.GetContentTypeByObjectName(studentReport.ObjectName),
                                        URL = await _firebaseService.GetUrlByObjectName(studentReport.ObjectName)
                                    }
                                }
                                : null,
                            HundredPercentReport = studentReport?.Progression == Progression.HundredPercent
                                ? new ReportFileResponseDto
                                {
                                    UploadedBy = studentReport.Teacher.Id,
                                    Progression = studentReport.Progression,
                                    File = new FilesResponseDto
                                    {
                                        FileName = studentReport.FileName,
                                        ContentType = await _firebaseService.GetContentTypeByObjectName(studentReport.ObjectName),
                                        URL = await _firebaseService.GetUrlByObjectName(studentReport.ObjectName)
                                    }
                                }
                                : null,
                            SpecialReport = studentReport?.Progression == Progression.Special
                                ? new ReportFileResponseDto
                                {
                                    UploadedBy = studentReport.Teacher.Id,
                                    Progression = studentReport.Progression,
                                    File = new FilesResponseDto
                                    {
                                        FileName = studentReport.FileName,
                                        ContentType = await _firebaseService.GetContentTypeByObjectName(studentReport.ObjectName),
                                        URL = await _firebaseService.GetUrlByObjectName(studentReport.ObjectName)
                                    }
                                }
                                : null
                        };

                        studySubjectDto.Members.Add(reportDto);
                    }

                    studyCourseDto.StudySubjects.Add(studySubjectDto);
                }

                response.Data.Add(studyCourseDto);
            }
            return response;
        }

        public async Task<ServiceResponse<StaffCoursesDetailResponseDto>> GetCourseDetail(int studyCourseId)
        {
            var response = new ServiceResponse<StaffCoursesDetailResponseDto>();

            var dbStudyCourse = await _context.StudyCourses
                                .Include(sc => sc.Course)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.Subject)
                                .Include(sc => sc.Level)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudyClasses)
                                        .ThenInclude(sc => sc.Schedule)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudyClasses)
                                        .ThenInclude(sc => sc.Teacher)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudyClasses)
                                        .ThenInclude(sc => sc.TeacherShifts)
                                .FirstOrDefaultAsync(sc => sc.Id == studyCourseId) ?? throw new NotFoundException("Course not found.");

            var data = new StaffCoursesDetailResponseDto
            {
                StudyCourseId = dbStudyCourse.Id,
                CourseId = dbStudyCourse.Course.Id,
                Course = dbStudyCourse.Course.course,
                Subjects = dbStudyCourse.StudySubjects.Select(dbStudySubject => new StudySubjectResponseDto
                {
                    StudySubjectId = dbStudySubject.Id,
                    SubjectId = dbStudySubject.Subject.Id,
                    Subject = dbStudySubject.Subject.subject,
                    Hour = dbStudySubject.Hour,
                }).ToList(),
                Level = new Dtos.LevelDtos.LevelResponseDto
                {
                    LevelId = dbStudyCourse.Level!.Id,
                    Level = dbStudyCourse.Level.level
                },
                Section = dbStudyCourse.Section,
                Method = dbStudyCourse.Method,
                StartDate = dbStudyCourse.StartDate.ToDateString(),
                EndDate = dbStudyCourse.EndDate.ToDateString(),
                TotalHour = dbStudyCourse.TotalHour,
                Status = dbStudyCourse.Status,
                Schedules = dbStudyCourse.StudySubjects.SelectMany(dbStudySubject => dbStudySubject.StudyClasses.Select(dbStudyClass => new ScheduleResponseDto
                {
                    Day = dbStudyClass.Schedule.Date.DayOfWeek.ToString().ToUpper(),
                    StudyCourseId = dbStudyCourse.Id,
                    CourseId = dbStudyCourse.Course.Id,
                    CourseName = dbStudyCourse.Course.course,
                    StudySubjectId = dbStudySubject.Id,
                    SubjectId = dbStudySubject.Subject.Id,
                    SubjectName = dbStudySubject.Subject.subject,
                    CourseSubject = dbStudyCourse.Course.course + " " + dbStudySubject.Subject.subject + " " + (dbStudyCourse.Level?.level ?? ""),
                    StudyClassId = dbStudyClass.Id,
                    ClassNo = dbStudyClass.ClassNumber,
                    Date = dbStudyClass.Schedule.Date.ToDateString(),
                    FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                    ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                    IsFiftyPercent = dbStudyClass.IsFiftyPercent,
                    IsHundredPercent = dbStudyClass.IsHundredPercent,
                    Teacher = new TeacherNameResponseDto
                    {
                        TeacherId = dbStudyClass.Teacher.Id,
                        FirstName = dbStudyClass.Teacher.FirstName,
                        LastName = dbStudyClass.Teacher.LastName,
                        Nickname = dbStudyClass.Teacher.Nickname,
                        FullName = dbStudyClass.Teacher.FullName,
                    },
                    ClassStatus = dbStudyClass.Status,
                    AdditionalHours = dbStudyClass.TeacherShifts
                                                .Where(x => x.TeacherWorkType != TeacherWorkType.NORMAL)
                                                .Select(shift => new AdditionalHours
                                                {
                                                    Hours = shift.Hours,
                                                    TeacherWorkType = shift.TeacherWorkType,
                                                })
                                                .FirstOrDefault()
                }))
                .OrderBy(s => (s.Date + " " + s.FromTime).ToDateTime())
                .ToList()
            };

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<StudySubjectMemberResponseDto>> GetStudyCourseMember(int studyCourseId)
        {
            var response = new ServiceResponse<StudySubjectMemberResponseDto>();

            var dbStudyCourse = await _context.StudyCourses
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.Subject)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudySubjectMember)
                                        .ThenInclude(sm => sm.Student)
                                .Include(sc => sc.StudySubjects)
                                    .ThenInclude(ss => ss.StudyClasses)
                                        .ThenInclude(sc => sc.Teacher)
                                .FirstOrDefaultAsync(sc => sc.Id == studyCourseId) ?? throw new NotFoundException("No course found.");

            var data = new StudySubjectMemberResponseDto
            {
                Students = dbStudyCourse.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sc => sc.Student != null)
                            .GroupBy(sm => sm.Student.Id)
                            .Select(group => new StudentStudySubjectMemberResponseDto
                            {
                                StudentId = group.First().Student.Id,
                                StudentCode = group.First().Student.StudentCode!,
                                StudentFirstName = group.First().Student.FirstName,
                                StudentLastName = group.First().Student.LastName,
                                StudentNickname = group.First().Student.Nickname,
                                Phone = group.First().Student.Phone, // Corrected phone retrieval
                                CourseJoinedDate = group.First().CourseJoinedDate.ToDateTimeString(),
                                Subjects = group.Select(member => new StudySubjectResponseDto
                                {
                                    StudySubjectId = member.StudySubject.Id,
                                    SubjectId = member.StudySubject.Subject.Id,
                                    Subject = member.StudySubject.Subject.subject,
                                    Hour = member.StudySubject.Hour,
                                }).ToList()
                            }).ToList(),

                Teachers = dbStudyCourse.StudySubjects
                            .SelectMany(ss => ss.StudyClasses)
                            .Where(sc => sc.Teacher != null)
                            .GroupBy(sc => sc.Teacher)
                            .Select(group => new TeacherStudySubjectMemberResponseDto
                            {
                                TeacherId = group.Key.Id,
                                TeacherFirstName = group.Key.FirstName,
                                TeacherLastName = group.Key.LastName,
                                TeacherNickname = group.Key.Nickname,
                                Phone = group.Key.Phone,
                                CourseJoinedDate = group.First().Schedule.Date.ToDateTimeString(),
                                Subjects = group.Select(cls => new StudySubjectResponseDto
                                {
                                    StudySubjectId = cls.StudySubject.Id,
                                    SubjectId = cls.StudySubject.Subject.Id,
                                    Subject = cls.StudySubject.Subject.subject,
                                    Hour = cls.StudySubject.Hour,
                                }).ToList()
                            }).ToList()
            };

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<string>> EaAddStudent(EaStudentManagementRequestDto requestDto)
        {
            var response = new ServiceResponse<string>();

            var dbStudyCourse = await _context.StudyCourses
                                .Include(sc => sc.Course)
                                .FirstOrDefaultAsync(sc => sc.Id == requestDto.StudyCourseId) ?? throw new NotFoundException("No Course Found.");

            var dbStudySubjects = await _context.StudySubjects
                                .Include(ss => ss.Subject)
                                .Include(ss => ss.StudyCourse)
                                .Include(ss => ss.StudySubjectMember)
                                .Include(ss => ss.StudyClasses)
                                    .ThenInclude(sc => sc.Attendances)
                                .Where(ss => requestDto.StudySubjectIds.Contains(ss.Id) && ss.StudyCourse.Id == requestDto.StudyCourseId)
                                .ToListAsync()
                                ?? throw new NotFoundException("No Subjects Found.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == requestDto.StudentId) ?? throw new NotFoundException("No Student Found.");

            var staffId = _firebaseService.GetAzureIdWithToken();
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == staffId) ?? throw new NotFoundException("No Staff Found.");

            var subjectList = new List<string>();

            foreach (var dbStudySubject in dbStudySubjects)
            {
                subjectList.Add(dbStudySubject.Subject.subject);

                var existingMember = await _context.StudySubjectMember.FirstOrDefaultAsync(sm => sm.Student.Id == student.Id && sm.StudySubject.Id == dbStudySubject.Id);

                if (existingMember != null)
                    throw new BadRequestException("The student is already in the subject");

                var member = new StudySubjectMember
                {
                    Student = student,
                    CourseJoinedDate = DateTime.Now,
                    Status = StudySubjectMemberStatus.Success
                };

                dbStudySubject.StudySubjectMember.Add(member);

                foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                {
                    var studentAttendance = new StudentAttendance
                    {
                        StudyClass = dbStudyClass,
                        Student = student,
                        Attendance = Attendance.None
                    };

                    dbStudyClass.Attendances.Add(studentAttendance);
                }
            }

            var history = new StudyCourseHistory
            {
                StudyCourse = dbStudyCourse,
                Staff = staff,
                UpdatedDate = DateTime.Now,
                Type = StudyCourseHistoryType.Member,
                Method = StudyCourseHistoryMethod.AddMember,
                Student = student,
            };

            string joinedSubject = string.Join(",", subjectList);
            string historyDescription = $"Added Student {student.FirstName} {student.LastName[0]}. ({student.Nickname}) to {dbStudyCourse.Course.course} {joinedSubject}.";

            history.Description = historyDescription;

            dbStudyCourse.StudyCourseHistories.Add(history);

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<string>> EaRemoveStudent(EaStudentManagementRequestDto requestDto)
        {
            var response = new ServiceResponse<string>();

            var dbStudyCourse = await _context.StudyCourses
                                .Include(sc => sc.Course)
                                .FirstOrDefaultAsync(sc => sc.Id == requestDto.StudyCourseId) ?? throw new NotFoundException("No Course Found.");

            var dbStudySubjects = await _context.StudySubjects
                                    .Include(ss => ss.Subject)
                                    .Include(ss => ss.StudyCourse)
                                    .Include(ss => ss.StudySubjectMember)
                                    .Include(ss => ss.StudyClasses)
                                        .ThenInclude(sc => sc.Attendances)
                                            .ThenInclude(a => a.Student)
                                    .Where(ss => requestDto.StudySubjectIds.Contains(ss.Id) && ss.StudyCourse.Id == requestDto.StudyCourseId)
                                    .ToListAsync()
                                    ?? throw new NotFoundException("No Subjects Found.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == requestDto.StudentId) ?? throw new NotFoundException("No Student Found.");

            var staffId = _firebaseService.GetAzureIdWithToken();
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == staffId) ?? throw new NotFoundException("No Staff Found.");

            var subjectList = new List<string>();

            foreach (var dbStudySubject in dbStudySubjects)
            {
                subjectList.Add(dbStudySubject.Subject.subject);

                var studentToRemove = dbStudySubject.StudySubjectMember
                                        .FirstOrDefault(sm => sm.Student.Id == requestDto.StudentId)
                                        ?? throw new NotFoundException("No Student Found");

                foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                {
                    var attendanceToRemove = dbStudyClass.Attendances
                                            .FirstOrDefault(a => a.Student!.Id == requestDto.StudentId)
                                            ?? throw new NotFoundException($"No Attendance with Student {requestDto.StudentId} Found.");

                    _context.StudentAttendances.Remove(attendanceToRemove);
                }

                _context.StudySubjectMember.Remove(studentToRemove);
            }

            var history = new StudyCourseHistory
            {
                StudyCourse = dbStudyCourse,
                Staff = staff,
                UpdatedDate = DateTime.Now,
                Type = StudyCourseHistoryType.Member,
                Method = StudyCourseHistoryMethod.RemoveMember,
                Student = student,
            };

            string joinedSubject = string.Join(",", subjectList);
            string historyDescription = $"Removed Student {student.FirstName} {student.LastName[0]}. ({student.Nickname}) from {dbStudyCourse.Course.course} {joinedSubject}.";

            history.Description = historyDescription;

            dbStudyCourse.StudyCourseHistories.Add(history);

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<ClassProgressResponseDto>> GetCourseProgress(int studyCourseId)
        {
            var response = new ServiceResponse<ClassProgressResponseDto>();

            var data = new ClassProgressResponseDto();

            var dbStudyCourse = await _context.StudyCourses
                                .Include(sc => sc.Course)
                                .FirstOrDefaultAsync(sc => sc.Id == studyCourseId)
                                ?? throw new NotFoundException("No Course Found.");

            var dbStudySubjects = await _context.StudySubjects
                                .Include(ss => ss.StudyCourse)
                                    .ThenInclude(sc => sc.Course)
                                .Include(ss => ss.Subject)
                                .Include(ss => ss.StudyClasses)
                                .Where(ss => ss.StudyCourse.Id == studyCourseId)
                                .ToListAsync()
                                ?? throw new NotFoundException("No Subject Found.");

            int completedClass = 0;
            int incompleteClass = 0;

            foreach (var dbStudySubject in dbStudySubjects)
            {
                foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                {
                    if (dbStudyClass.Status == ClassStatus.CHECKED || dbStudyClass.Status == ClassStatus.UNCHECKED)
                    {
                        completedClass += 1;
                    }
                    else if (dbStudyClass.Status == ClassStatus.NONE)
                    {
                        incompleteClass += 1;
                    }
                }
            }

            double progressRatio = incompleteClass != 0 ? (double)completedClass / incompleteClass : 0;
            double progress = Math.Round(progressRatio * 100);

            data.StudyCourseId = dbStudyCourse.Id;
            data.CourseId = dbStudyCourse.Course.Id;
            data.Course = dbStudyCourse.Course.course;
            data.Progress = $"{progress}%";

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateScheduleWithoutCancelRequest(UpdateStudyCourseRequestDto updateRequest)
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
                                        .Where(c => updateRequest.RemoveStudyClassId.Contains(c.Id)).ToListAsync();

            foreach (var dbRemoveStudyClass in dbRemoveStudyClasses)
            {
                foreach (var dbAttendance in dbRemoveStudyClass.Attendances)
                {
                    _context.StudentAttendances.Remove(dbAttendance);
                }
                dbRemoveStudyClass.Status = ClassStatus.DELETED;
                dbRemoveStudyClass.Schedule.CalendarType = DailyCalendarType.DELETED;

                var removeHistory = new StudyCourseHistory
                {
                    StudyCourse = dbRemoveStudyClass.StudyCourse,
                    Staff = staff,
                    UpdatedDate = DateTime.Now,
                    Type = StudyCourseHistoryType.Schedule,
                    Method = StudyCourseHistoryMethod.RemoveSchedule,
                    StudyClass = dbRemoveStudyClass
                };

                string removedStudyClassHistoryDescription = $"Removed {dbRemoveStudyClass.StudyCourse.Course.course} {dbRemoveStudyClass.StudySubject.Subject.subject} on {dbRemoveStudyClass.Schedule.Date.ToDateWithDayString()} ({dbRemoveStudyClass.Schedule.FromTime.ToTimeSpanString()} - {dbRemoveStudyClass.Schedule.ToTime.ToTimeSpanString()}) taught by Teacher {dbRemoveStudyClass.Teacher.Nickname}.";

                removeHistory.Description = removedStudyClassHistoryDescription;
                dbRemoveStudyClass.StudyCourse.StudyCourseHistories.Add(removeHistory);
            }

            var dbStudySubjects = await _context.StudySubjects
                            .Include(s => s.StudyCourse)
                                .ThenInclude(sc => sc.Course)
                            .Include(s => s.Subject)
                            .Include(s => s.StudyClasses)
                            .Include(s => s.StudyCourse)
                            .Include(s => s.StudySubjectMember)
                                .ThenInclude(s => s.Student)
                            .Where(s => updateRequest.StudySubjectIds.Contains(s.Id))
                            .ToListAsync();

            var dbTeachers = await _context.Teachers
                                           .Include(x => x.Mandays)
                                                .ThenInclude(x => x.WorkTimes)
                                           .ToListAsync();

            foreach (var dbStudySubject in dbStudySubjects)
            {
                foreach (var newSchedule in updateRequest.NewSchedule.Where(s => s.StudySubjectId == dbStudySubject.Id))
                {
                    var classCount = dbStudySubject.StudyClasses.Count;
                    var dbTeacher = dbTeachers.FirstOrDefault(t => t.Id == newSchedule.TeacherId)
                                ?? throw new NotFoundException($"Teacher ID {newSchedule.TeacherId} is not found.");

                    int studyClassCount = updateRequest.NewSchedule.Where(x => x.StudySubjectId == dbStudySubject.Id).Count();
                    int c = 0;

                    var studyClass = new StudyClass
                    {
                        //TODO Class Count
                        Teacher = dbTeacher,
                        StudyCourse = dbStudySubject.StudyCourse,
                        Schedule = new Schedule
                        {
                            Date = newSchedule.Date.ToDateTime(),
                            FromTime = newSchedule.FromTime.ToTimeSpan(),
                            ToTime = newSchedule.ToTime.ToTimeSpan(),
                            Type = ScheduleType.Class,
                        },
                    };

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

                    var worktypes = _teacherService.GetTeacherWorkTypesWithHours(dbTeacher, newSchedule.Date.ToDateTime(), newSchedule.FromTime.ToTimeSpan(), newSchedule.ToTime.ToTimeSpan());
                    foreach (var worktype in worktypes)
                    {
                        studyClass.TeacherShifts.Add(new TeacherShift
                        {
                            Teacher = dbTeacher,
                            TeacherWorkType = worktype.TeacherWorkType,
                            Hours = worktype.Hours,
                        });
                    }

                    foreach (var dbMember in dbStudySubject.StudySubjectMember)
                    {
                        studyClass.Attendances.Add(new StudentAttendance
                        {
                            Attendance = Attendance.None,
                            Student = dbMember.Student,
                        });
                    }

                    dbStudySubject.StudyClasses.Add(studyClass);

                    var addHistory = new StudyCourseHistory
                    {
                        StudyCourse = dbStudySubject.StudyCourse,
                        Staff = staff,
                        UpdatedDate = DateTime.Now,
                        Type = StudyCourseHistoryType.Schedule,
                        Method = StudyCourseHistoryMethod.AddSchedule,
                        StudyClass = studyClass
                    };

                    string addedStudyClassHistoryDescription = $"Added {dbStudySubject.StudyCourse.Course.course} {dbStudySubject.Subject.subject} on {newSchedule.Date.ToDateTime().ToDateWithDayString()} ({newSchedule.FromTime.ToTimeSpan().ToTimeSpanString()} - {newSchedule.ToTime.ToTimeSpan().ToTimeSpanString()}) taught by Teacher {dbTeachers.FirstOrDefault(t => t.Id == newSchedule.TeacherId)!.Nickname}.";

                    addHistory.Description = addedStudyClassHistoryDescription;
                    studyClass.StudyCourse.StudyCourseHistories.Add(addHistory);
                }
            }

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<List<TodayClassMobileResponseDto>>> GetMobileTodayClass(string requestDate)
        {
            var userId = _firebaseService.GetAzureIdWithToken();
            var role = _firebaseService.GetRoleWithToken();
            List<StudyClass> dbStudyClasses = new();
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
                                && c.TeacherId == userId
                                && c.StudyCourse.Status != StudyCourseStatus.Pending
                                && c.StudyCourse.Status != StudyCourseStatus.Cancelled)
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
                                && c.StudySubject.StudySubjectMember.Any(m => m.StudentId == userId && m.Status == StudySubjectMemberStatus.Success))
                                .ToListAsync();
            }
            else
            {
                throw new InternalServerException("Something went wrong with User.");
            }


            var data = new List<TodayClassMobileResponseDto>();
            foreach (var dbStudyClass in dbStudyClasses)
            {
                var studyClass = new TodayClassMobileResponseDto
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
                    Date = dbStudyClass.Schedule.Date.ToDateString(),
                    FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                    ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                    Room = dbStudyClass.Schedule.Room,
                    StudyCourseType = dbStudyClass.StudyCourse.StudyCourseType,
                    TeacherId = dbStudyClass.Teacher.Id,
                    TeacherFirstName = dbStudyClass.Teacher.FirstName,
                    TeacherLastName = dbStudyClass.Teacher.LastName,
                    TeacherNickname = dbStudyClass.Teacher.Nickname,
                };
                data.Add(studyClass);
            }

            var response = new ServiceResponse<List<TodayClassMobileResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
            return response;
        }

        public async Task<ServiceResponse<List<StudyCourseHistoryResponseDto>>> GetStudyCourseHistory(int studyCourseId)
        {
            var response = new ServiceResponse<List<StudyCourseHistoryResponseDto>>();

            var dbStudyCourseHistories = await _context.StudyCourseHistories
                                        .Include(sch => sch.Staff)
                                        .Where(sch => sch.StudyCourse.Id == studyCourseId)
                                        .ToListAsync()
                                        ?? throw new NotFoundException("No Records Found.");

            var data = dbStudyCourseHistories.Select(history => new StudyCourseHistoryResponseDto
            {
                Date = history.UpdatedDate.ToDateTimeString(),
                RecordType = history.Type,
                Record = $"[{history.Staff.Role.ToUpper()} {history.Staff.Nickname}/{history.Staff.FirstName}] {history.Description}"
            })
            .OrderByDescending(sch => sch.Date.ToDateTime())
            .ToList();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<string>> CancelStudyCourse(int studyCourseId)
        {
            var dbStudyCourse = await _context.StudyCourses
                                .Include(c => c.StudyClasses)
                                    .ThenInclude(c => c.Schedule)
                                .FirstOrDefaultAsync(c => c.Id == studyCourseId)
                                ?? throw new NotFoundException($"StudyCourse with ID {studyCourseId} is not found.");

            dbStudyCourse.Status = StudyCourseStatus.Cancelled;
            foreach (var dbStudyClass in dbStudyCourse.StudyClasses)
            {
                dbStudyClass.Status = ClassStatus.DELETED;
                dbStudyClass.Schedule.CalendarType = DailyCalendarType.DELETED;
            }

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>()
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }
    }
}