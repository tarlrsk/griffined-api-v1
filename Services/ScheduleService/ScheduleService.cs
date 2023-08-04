using Extensions.DateTimeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using griffined_api.Dtos.ScheduleDtos;

namespace griffined_api.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFirebaseService _firebaseService;
        public ScheduleService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
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
                throw new BadRequestException($"Course or Level is not found");
            }

            var studyCourse = new StudyCourse();

            studyCourse.Course = dbCourse;
            studyCourse.Level = dbCourse.Levels.First();
            studyCourse.Section = newRequestedSchedule.Section;
            studyCourse.TotalHour = newRequestedSchedule.TotalHours;
            studyCourse.StartDate = DateTime.Parse(newRequestedSchedule.StartDate);
            studyCourse.EndDate = DateTime.Parse(newRequestedSchedule.EndDate);
            studyCourse.StudyCourseType = StudyCourseType.Group;
            studyCourse.Method = newRequestedSchedule.Method;
            studyCourse.Status = CourseStatus.NotStarted;

            var teachers = await _context.Teachers.ToListAsync();

            foreach (var newStudySubject in dbCourse.Subjects)
            {
                var studySubject = new StudySubject();
                var classNumber = 1;
                foreach (var newSchedule in newRequestedSchedule.Schedules)
                {
                    if (newSchedule.SubjectId == newStudySubject.Id)
                    {
                        var teacher = teachers.FirstOrDefault(t => t.Id == newSchedule.TeacherId);
                        if (teacher == null)
                            throw new BadRequestException($"Teacher with ID {newSchedule.TeacherId} is not found.");

                        var studyClass = new StudyClass()
                        {
                            isMakeup = false,
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
                                        .Include(c => c.StudySubjects)
                                            .ThenInclude(s => s.Subject)
                                        .Include(c => c.Course)
                                        .Include(c => c.Level)
                                        .ToListAsync();

            var studyCourses = new List<StudyCourseResponseDto>();
            foreach (var dbStudyCourse in dbStudyCourses)
            {
                var studyCourse = new StudyCourseResponseDto();
                studyCourse.StudyCourseId = dbStudyCourse.Id;
                studyCourse.Section = dbStudyCourse.Section;
                studyCourse.Course = dbStudyCourse.Course.course;
                studyCourse.Level = dbStudyCourse.Level?.level;
                studyCourse.TotalHour = dbStudyCourse.TotalHour;
                studyCourse.StartDate = dbStudyCourse.StartDate.ToString("dd-MMMM-yyyy");
                studyCourse.EndDate = dbStudyCourse.EndDate.ToString("dd-MMMM-yyyy");
                studyCourse.Method = dbStudyCourse.Method;
                studyCourse.StudyCourseType = dbStudyCourse.StudyCourseType;
                studyCourse.CourseStatus = dbStudyCourse.Status;

                var studentCount = 0;
                var student = new List<int>();
                foreach (var dbStudySubject in dbStudyCourse.StudySubjects)
                {
                    foreach (var dbMember in dbStudySubject.StudySubjectMember)
                    {
                        if (!(student.Exists(s => s == dbMember.StudentId)))
                        {
                            studentCount += 1;
                            student.Add(dbMember.StudentId);
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
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                            ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                            CourseSubject = dbStudyCourse.Course.course + " " + dbStudySubject.Subject.subject + " " + (dbStudyCourse.Level?.level ?? ""),
                            TeacherId = dbStudyClass.Teacher.Id,
                            TeacherFirstName = dbStudyClass.Teacher.FirstName,
                            TeacherLastName = dbStudyClass.Teacher.LastName,
                            TeacherNickName = dbStudyClass.Teacher.Nickname,
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

        public async Task<ServiceResponse<String>> AddNewStudyClass(List<NewStudyClassScheduleRequestDto> newStudyClasses, int requestId)
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
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingEA);

            if (dbRequest == null)
                throw new BadRequestException($"Pending EA Request with ID {requestId} is not found.");

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
                    Status = CourseStatus.Pending,
                    NewCourseRequest = dbNewRequestedCourse,
                };
                foreach (var dbNewRequestedSubject in dbNewRequestedCourse.NewCourseSubjectRequests)
                {
                    var studySubject = new StudySubject()
                    {
                        Subject = dbNewRequestedSubject.Subject
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
                    var requestedStudyClasses = newStudyClasses.Where(c => c.SubjectId == dbNewRequestedSubject.Id && c.CourseId == dbNewRequestedCourse.Id);
                    foreach (var requestedStudyClass in requestedStudyClasses)
                    {
                        var dbTeacher = dbTeachers.FirstOrDefault(t => t.Id == requestedStudyClass.TeacherId);
                        if (dbTeacher == null)
                            throw new BadRequestException($"Teacher with ID {requestedStudyClass.TeacherId} is not found.");

                        var studyClass = new StudyClass()
                        {
                            isMakeup = false,
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
            dbRequest.ByEAId = _firebaseService.GetAzureIdWithToken();
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<String>();
            response.StatusCode = (int)HttpStatusCode.OK; ;
            return response;
        }
    }
}