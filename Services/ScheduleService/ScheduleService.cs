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
    }
}