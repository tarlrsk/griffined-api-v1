using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using griffined_api.Dtos.LevelDtos;
using griffined_api.Dtos.SubjectDtos;

namespace griffined_api.Services.CourseService
{
    public class CourseService : ICourseService
    {
        private readonly DataContext _context;
        public CourseService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<CourseResponseDto>>> ListAllCourseSubjectLevel()
        {
            var response = new ServiceResponse<List<CourseResponseDto>>();

            var dbCourses = await _context.Courses
                                    .Include(c => c.Subjects)
                                    .Include(c => c.Levels)
                                    .ToListAsync();

            var data = new List<CourseResponseDto>();

            foreach (var course in dbCourses)
            {
                var courseResponse = new CourseResponseDto
                {
                    CourseId = course.Id,
                    Course = course.course,
                    Subjects = course.Subjects.Select(subject => new SubjectResponseDto
                    {
                        SubjectId = subject.Id,
                        Subject = subject.subject
                    }).ToList(),
                    Levels = course.Levels.Select(level => new LevelResponseDto
                    {
                        LevelId = level.Id,
                        Level = level.level
                    }).GroupBy(l => l.Level).Select(group => group.First()).ToList()
                };

                data.Add(courseResponse);
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }
    }
}

