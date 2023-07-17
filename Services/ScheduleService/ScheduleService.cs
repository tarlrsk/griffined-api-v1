using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.ScheduleDtos;

namespace griffined_api.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ScheduleService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _context = context;
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
                foreach (var newSchedule in newRequestedSchedule.schedules)
                {
                    if (newSchedule.SubjectId == newStudySubject.Id)
                    {
                        var schedule = new Schedule()
                        {
                            Date = newSchedule.Date,
                            FromTime = newSchedule.FromTime,
                            ToTime = newSchedule.ToTime,
                            Type = ScheduleType.Class,
                        };

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
                                Date = newSchedule.Date,
                                FromTime = newSchedule.FromTime,
                                ToTime = newSchedule.ToTime,
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
            response.StatusCode = 200;
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
                                            .ThenInclude(s => s.CourseMembers)
                                        .Include(c => c.StudySubjects)
                                            .ThenInclude(s => s.Subject)
                                        .Include(c => c.Course)
                                        .Include(c => c.Level)
                                        .Where(c => c.Status == CourseStatus.Ongoing || c.Status == CourseStatus.NotStarted)
                                        .ToListAsync();

            var studyCourses = new List<StudyCourseResponseDto>();
            foreach (var dbStudyCourse in dbStudyCourses)
            {
                var studyCourse = new StudyCourseResponseDto();
                studyCourse.StudyCourseId = dbStudyCourse.Id;
                studyCourse.Section = dbStudyCourse.Section;
                studyCourse.Course = dbStudyCourse.Course.course;
                studyCourse.Level = dbStudyCourse.Level.level;
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
                    foreach (var dbMember in dbStudySubject.CourseMembers)
                    {
                        if (!(student.Exists(s => s == dbMember.StudentId)))
                        {
                            studentCount += 1;
                            student.Add(dbMember.StudentId);
                        }
                    }
                    studyCourse.StudySubjects.Add(dbStudySubject.Subject.subject);
                    foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                    {
                        var schedule = new ScheduleResponseDto()
                        {
                            Date = dbStudyClass.Schedule.Date,
                            FromTime = dbStudyClass.Schedule.FromTime,
                            ToTime = dbStudyClass.Schedule.ToTime,
                            CourseSubject = dbStudyCourse.Course.course + " " + dbStudySubject.Subject.subject + " " + dbStudyCourse.Level.level,
                            TeacherFirstName = dbStudyClass.Teacher.FirstName,
                            TeacherLastName = dbStudyClass.Teacher.LastName,
                            TeacherNickName = dbStudyClass.Teacher.Nickname,
                            // TODO Teacher Work Type
                        };
                        studyCourse.Schedule.Add(schedule);
                    }
                }

                studyCourse.StudentCount = studentCount;
                studyCourses.Add(studyCourse);
            }
            response.Data = studyCourses;
            response.StatusCode = 200;
            return response;
        }
    }
}