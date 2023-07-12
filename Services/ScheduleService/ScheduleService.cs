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
        public async Task<ServiceResponse<string>> AddGroupSchedule(GroupScheduleRequestDto newSchedule)
        {
            var response = new ServiceResponse<string>();

            var dbCourse = await _context.Courses
                            .Include(c => c.Subjects.Where(s => newSchedule.SubjectIds.Contains(s.Id)))
                            .Include(c => c.Levels.FirstOrDefault(l => l.Id == newSchedule.LevelId))
                            .FirstOrDefaultAsync(c => c.Id == newSchedule.CourseId);
            if (dbCourse == null || dbCourse.Levels == null)
            {
                throw new BadRequestException($"Course or Level is not found");
            }
            
            var studyCourse = new StudyCourse();

            foreach (var newStudySubject in newSchedule.SubjectIds)
            {
                studyCourse.Course = dbCourse;
            }

            return response;
        }
    }
}