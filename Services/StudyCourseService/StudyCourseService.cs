using AutoMapper.Execution;
using griffined_api.Dtos.StudentReportDtos;
using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Extensions.DateTimeExtensions;
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
                        Subject = dbStudySubject.Subject.subject,
                    });
                    foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                    {
                        var schedule = new ScheduleResponseDto()
                        {
                            Room = null,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                            ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                            CourseSubject = dbStudyCourse.Course.course + " " + dbStudySubject.Subject.subject + " " + (dbStudyCourse.Level?.level ?? ""),
                            CourseId = dbStudyCourse.Course.Id,
                            CourseName = dbStudyCourse.Course.course,
                            SubjectId = dbStudySubject.Subject.Id,
                            SubjectName = dbStudySubject.Subject.subject,
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
        public async Task<ServiceResponse<StudyCourseDetailMobileResponseDto>> StudyCourseDetailForTeacher(int studyCourseId)
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

            var data = new StudyCourseDetailMobileResponseDto()
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
                        CourseId = dbStudyCourse.Course.Id,
                        CourseName = dbStudyCourse.Course.course,
                        SubjectId = dbStudySubject.Subject.Id,
                        SubjectName = dbStudySubject.Subject.subject,
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

            var response = new ServiceResponse<StudyCourseDetailMobileResponseDto>
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
            var dbCourses = await _context.StudyCourses
                                .Include(sc => sc.Course)
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

            var fiftyPercentReport = dbCourses.SelectMany(sc => sc.StudySubjects)
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .SelectMany(sm => sm.StudentReports)
                            .FirstOrDefault(sr => sr.Progression == Progression.FiftyPercent);

            var hundredPercentReport = dbCourses.SelectMany(sc => sc.StudySubjects)
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .SelectMany(sm => sm.StudentReports)
                            .FirstOrDefault(sr => sr.Progression == Progression.HundredPercent);

            var specialReport = dbCourses.SelectMany(sc => sc.StudySubjects)
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .SelectMany(sm => sm.StudentReports)
                            .FirstOrDefault(sr => sr.Progression == Progression.Special);

            var reportDto = dbCourses
                            .SelectMany(sc => sc.StudySubjects)
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.StudentReports)
                            .FirstOrDefault()!
                            .Select(async report => new StudySubjectReportResponseDto
                            {
                                StudySubject = new StudySubjectResponseDto
                                {
                                    StudySubjectId = report.StudySubjectMember.StudySubject.Id,
                                    Subject = report.StudySubjectMember.StudySubject.Subject.subject
                                },
                                FiftyPercentReport = fiftyPercentReport != null
                                ? new ReportFileResponseDto
                                {
                                    UploadedBy = report.Teacher.Id,
                                    Progression = Progression.FiftyPercent,
                                    File = new FilesResponseDto
                                    {
                                        FileName = fiftyPercentReport.FileName,
                                        ContentType = await _firebaseService.GetContentTypeByObjectName(fiftyPercentReport.ObjectName),
                                        URL = await _firebaseService.GetUrlByObjectName(fiftyPercentReport.ObjectName)
                                    }
                                }
                                : null,
                                HundredPercentReport = hundredPercentReport != null
                                ? new ReportFileResponseDto
                                {
                                    UploadedBy = report.Teacher.Id,
                                    Progression = Progression.HundredPercent,
                                    File = new FilesResponseDto
                                    {
                                        FileName = hundredPercentReport.FileName,
                                        ContentType = await _firebaseService.GetContentTypeByObjectName(hundredPercentReport.ObjectName),
                                        URL = await _firebaseService.GetUrlByObjectName(hundredPercentReport.ObjectName)
                                    }
                                }
                                : null,
                                SpecialReport = specialReport != null
                                ? new ReportFileResponseDto
                                {
                                    UploadedBy = report.Teacher.Id,
                                    Progression = Progression.Special,
                                    File = new FilesResponseDto
                                    {
                                        FileName = specialReport.FileName,
                                        ContentType = await _firebaseService.GetContentTypeByObjectName(specialReport.ObjectName),
                                        URL = await _firebaseService.GetUrlByObjectName(specialReport.ObjectName)
                                    }
                                }
                                : null
                            }).ToList();

            var reportDtoList = await Task.WhenAll(reportDto);

            var data = dbCourses.Select(course => new StudyCourseByStudentIdResponseDto
            {
                StudentId = course.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.Id)
                            .FirstOrDefault()!,
                StudentCode = course.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.StudentCode)
                            .FirstOrDefault()!,
                StudentFirstName = course.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.FirstName)
                            .FirstOrDefault()!,
                StudentLastName = course.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.LastName)
                            .FirstOrDefault()!,
                StudentNickname = course.StudySubjects
                            .SelectMany(ss => ss.StudySubjectMember)
                            .Where(sm => sm.Student.StudentCode == studentCode)
                            .Select(sm => sm.Student.Nickname)
                            .FirstOrDefault()!,
                CourseId = course.Id,
                Course = course.Course.course,
                Status = course.Status,
                Reports = reportDtoList.ToList()
            }).ToList();

            var response = new ServiceResponse<List<StudyCourseByStudentIdResponseDto>>()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data
            };

            return response;
        }
    }
}