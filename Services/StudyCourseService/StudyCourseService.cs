using AutoMapper.Execution;
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
        public StudyCourseService(DataContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }
        public async Task<ServiceResponse<string>> AddGroupSchedule(GroupScheduleRequestDto newRequestedSchedule)
        {
            var response = new ServiceResponse<string>();

            var dbCourse = await _context.Courses
                            .Include(c => c.Subjects.Where(s => newRequestedSchedule.SubjectIds.Contains(s.Id)))
                            .Include(c => c.Levels.Where(l => l.Id == newRequestedSchedule.LevelId))
                            .FirstOrDefaultAsync(c => c.Id == newRequestedSchedule.CourseId);

            if (dbCourse == null || dbCourse.Levels == null)
            {
                throw new NotFoundException($"Course or Level is not found");
            }

            var studyCourse = new StudyCourse
            {
                Course = dbCourse,
                Level = dbCourse.Levels.First(),
                Section = newRequestedSchedule.Section,
                TotalHour = newRequestedSchedule.TotalHours,
                StartDate = DateTime.Parse(newRequestedSchedule.StartDate),
                EndDate = DateTime.Parse(newRequestedSchedule.EndDate),
                StudyCourseType = StudyCourseType.Group,
                Method = newRequestedSchedule.Method,
                Status = StudyCourseStatus.NotStarted
            };

            var teachers = await _context.Teachers.ToListAsync();

            foreach (var newStudySubject in dbCourse.Subjects)
            {
                var studySubject = new StudySubject();
                var classNumber = 1;
                foreach (var newSchedule in newRequestedSchedule.Schedules)
                {
                    if (newSchedule.SubjectId == newStudySubject.Id)
                    {
                        var teacher = teachers.FirstOrDefault(t => t.Id == newSchedule.TeacherId) ?? throw new NotFoundException($"Teacher with ID {newSchedule.TeacherId} is not found.");

                        var studyClass = new StudyClass()
                        {
                            IsMakeup = false,
                            ClassNumber = classNumber,
                            Teacher = teacher,
                            Schedule = new Schedule()
                            {
                                Date = newSchedule.Date.ToDateTime(),
                                FromTime = newSchedule.FromTime.ToTimeSpan(),
                                ToTime = newSchedule.ToTime.ToTimeSpan(),
                                Type = ScheduleType.Class,
                            }
                        };
                        classNumber = +1;
                        studySubject.StudyClasses.Add(studyClass);
                    }
                }
                studySubject.Subject = newStudySubject;
                studyCourse.StudySubjects.Add(studySubject);
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
                    StartDate = dbStudyCourse.StartDate.ToString("dd-MMMM-yyyy"),
                    EndDate = dbStudyCourse.EndDate.ToString("dd-MMMM-yyyy"),
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
                                StudentCode = dbMember.Student.StudentCode,
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
                    });
                    foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                    {
                        var schedule = new ScheduleResponseDto()
                        {
                            StudyCourseId = dbStudyCourse.Id,
                            CourseId = dbStudyCourse.Course.Id,
                            Course = dbStudyCourse.Course.course,
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                            CourseSubject = dbStudyCourse.Course.course + " " + dbStudySubject.Subject.subject + " " + (dbStudyCourse.Level?.level ?? ""),
                            StudyClassId = dbStudyClass.Id,
                            ClassNo = dbStudyClass.ClassNumber,
                            Room = null,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                            ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                            TeacherId = dbStudyClass.Teacher.Id,
                            TeacherFirstName = dbStudyClass.Teacher.FirstName,
                            TeacherLastName = dbStudyClass.Teacher.LastName,
                            TeacherNickname = dbStudyClass.Teacher.Nickname,
                            // TODO Teacher Work Type
                        };
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

            var dbTeachers = await _context.Teachers.ToListAsync();

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
                    foreach (var requestedStudyClass in requestedStudyClasses)
                    {
                        var dbTeacher = dbTeachers.FirstOrDefault(t => t.Id == requestedStudyClass.TeacherId) ?? throw new NotFoundException($"Teacher with ID {requestedStudyClass.TeacherId} is not found.");

                        var studyClass = new StudyClass()
                        {
                            IsMakeup = false,
                            ClassNumber = requestedStudyClass.ClassNo,
                            Teacher = dbTeacher,
                            Schedule = new Schedule()
                            {
                                Date = requestedStudyClass.Date.ToDateTime(),
                                FromTime = requestedStudyClass.FromTime.ToTimeSpan(),
                                ToTime = requestedStudyClass.ToTime.ToTimeSpan(),
                                Type = ScheduleType.Class,
                            }
                        };
                        studySubject.StudyClasses.Add(studyClass);
                    }
                    studyCourse.StudySubjects.Add(studySubject);
                }
                _context.StudyCourses.Add(studyCourse);
            }

            dbRequest.RegistrationStatus = RegistrationStatus.PendingEC;
            dbRequest.ScheduledByStaffId = _firebaseService.GetAzureIdWithToken();
            dbRequest.HasSchedule = true;
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<String> { StatusCode = (int)HttpStatusCode.OK };
            ;
            return response;
        }

        public async Task<ServiceResponse<string>> EditStudyClassByRegisRequest(EditStudyClassByRegistrationRequestDto requestDto, int requestId)
        {
            if (requestDto.ClassToDelete.Count != 0)
            {
                var dbStudyClasses = await _context.StudyClasses.Where(s => requestDto.ClassToDelete.Contains(s.Id)).ToListAsync();
                foreach (var dbStudyClass in dbStudyClasses)
                {
                    dbStudyClass.Status = ClassStatus.Deleted;
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

                        foreach (var newStudyClass in newStudyClasses)
                        {
                            var studyClass = new StudyClass
                            {
                                ClassNumber = newStudyClass.ClassNo,
                                Teacher = dbTeachers.FirstOrDefault(t => t.Id == newStudyClass.TeacherId) ?? throw new Exception($"Cannot Find Teacher ID {newStudyClass.TeacherId}"),
                                Schedule = new Schedule
                                {
                                    Date = newStudyClass.Date.ToDateTime(),
                                    FromTime = newStudyClass.FromTime.ToTimeSpan(),
                                    ToTime = newStudyClass.ToTime.ToTimeSpan(),
                                    Type = ScheduleType.Class,
                                }
                            };
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
                                .Include(m => m.StudySubject)
                                    .ThenInclude(s => s.Subject)
                                .Where(s => s.TeacherId == teacherId)
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
                });
                foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                {
                    schedules.Add(new ScheduleResponseDto
                    {
                        StudyClassId = dbStudyClass.Id,
                        ClassNo = dbStudyClass.ClassNumber,
                        Date = dbStudyClass.Schedule.Date.ToDateString(),
                        FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                        ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                        StudyCourseId = dbStudyCourse.Id,
                        CourseId = dbStudyCourse.Course.Id,
                        Course = dbStudyCourse.Course.course,
                        StudySubjectId = dbStudySubject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject,
                        CourseSubject = dbStudyCourse.Course.course + " "
                                            + dbStudySubject.Subject.subject
                                            + " " + (dbStudyCourse.Level?.level ?? ""),
                        ClassStatus = dbStudyClass.Status,
                        TeacherId = dbStudyClass.Teacher.Id,
                        TeacherFirstName = dbStudyClass.Teacher.FirstName,
                        TeacherLastName = dbStudyClass.Teacher.LastName,
                        TeacherNickname = dbStudyClass.Teacher.Nickname,
                        // TODO WorkType
                    });
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
                                .Where(c => dbStudySubjects.Contains(c.StudySubject))
                                .Select(c => new
                                {
                                    StudyClass = c,
                                    c.StudySubject,
                                    c.Schedule,
                                    Attendance = c.Attendances.FirstOrDefault(a => a.StudentId == studentId),
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
                    // TODO WorkType
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
            var dbClass = await _context.StudyClasses.FirstOrDefaultAsync(c => c.Id == studyClassId) ?? throw new NotFoundException($"Class with ID {studyClassId} not found.");

            dbClass.Room = room;

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
                            Subject = dbStudySubject.Subject.subject
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
                            StudentCode = dbStudySubjectMember.Student.StudentCode,
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
                                .FirstOrDefaultAsync(sc => sc.Id == studyCourseId) ?? throw new NotFoundException("Course not found.");

            var data = new StaffCoursesDetailResponseDto
            {
                StudyCourseId = dbStudyCourse.Id,
                Course = dbStudyCourse.Course.course,
                Subjects = dbStudyCourse.StudySubjects.Select(dbStudySubject => new StudySubjectResponseDto
                {
                    StudySubjectId = dbStudySubject.Id,
                    SubjectId = dbStudySubject.Subject.Id,
                    Subject = dbStudySubject.Subject.subject
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
                Schedules = dbStudyCourse.StudySubjects.SelectMany(dbStudySubject => dbStudySubject.StudyClasses.Select(dbStudyClass => new ScheduleResponseDto
                {
                    StudyCourseId = dbStudyCourse.Id,
                    CourseId = dbStudyCourse.Course.Id,
                    Course = dbStudyCourse.Course.course,
                    StudySubjectId = dbStudySubject.Id,
                    SubjectId = dbStudySubject.Subject.Id,
                    Subject = dbStudySubject.Subject.subject,
                    CourseSubject = dbStudyCourse.Course.course + " " + dbStudySubject.Subject.subject + " " + (dbStudyCourse.Level?.level ?? ""),
                    StudyClassId = dbStudyClass.Id,
                    ClassNo = dbStudyClass.ClassNumber,
                    Date = dbStudyClass.Schedule.Date.ToDateString(),
                    FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                    ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                    TeacherNickname = dbStudyClass.Teacher.Nickname
                    // TODO TeacherWorkType
                })).ToList()
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
                                StudentFirstName = group.First().Student.FirstName,
                                StudentLastName = group.First().Student.LastName,
                                StudentNickname = group.First().Student.Nickname,
                                Phone = group.First().Student.Phone, // Corrected phone retrieval
                                CourseJoinedDate = group.First().CourseJoinedDate.ToDateTimeString(),
                                Subjects = group.Select(member => new StudySubjectResponseDto
                                {
                                    StudySubjectId = member.StudySubject.Id,
                                    SubjectId = member.StudySubject.Subject.Id,
                                    Subject = member.StudySubject.Subject.subject
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
                                    Subject = cls.StudySubject.Subject.subject
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

            var dbStudySubjects = await _context.StudySubjects
                                .Include(ss => ss.StudyCourse)
                                .Include(ss => ss.StudySubjectMember)
                                .Include(ss => ss.StudyClasses)
                                    .ThenInclude(sc => sc.Attendances)
                                .Where(ss => requestDto.StudySubjectIds.Contains(ss.Id) && ss.StudyCourse.Id == requestDto.StudyCourseId)
                                .ToListAsync()
                                ?? throw new NotFoundException("No Subjects Found.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == requestDto.StudentId) ?? throw new NotFoundException("No Student Found.");

            foreach (var dbStudySubject in dbStudySubjects)
            {
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

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<string>> EaRemoveStudent(EaStudentManagementRequestDto requestDto)
        {
            var response = new ServiceResponse<string>();

            var dbStudySubjects = await _context.StudySubjects
                                    .Include(ss => ss.StudyCourse)
                                    .Include(ss => ss.StudySubjectMember)
                                    .Include(ss => ss.StudyClasses)
                                        .ThenInclude(sc => sc.Attendances)
                                            .ThenInclude(a => a.Student)
                                    .Where(ss => requestDto.StudySubjectIds.Contains(ss.Id) && ss.StudyCourse.Id == requestDto.StudyCourseId)
                                    .ToListAsync()
                                    ?? throw new NotFoundException("No Subjects Found.");

            foreach (var dbStudySubject in dbStudySubjects)
            {
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

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }
    }
}