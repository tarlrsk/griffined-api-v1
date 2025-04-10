using Quartz;

namespace griffined_api.Jobs.BeginStudyCourse
{
    [DisallowConcurrentExecution]
    public class BeginStudyCourseJob : IJob
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _uow;
        public BeginStudyCourseJob(DataContext context, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var studyCourses = await _context.StudyCourses
                                  .Include(sc => sc.StudySubjects)
                                      .ThenInclude(ss => ss.StudyClasses)
                                  .Where(x => x.Status == StudyCourseStatus.NotStarted)
                                  .ToListAsync();

            _uow.BeginTran();

            foreach (var studyCourse in studyCourses)
            {
                if (DateTime.UtcNow.AddHours(7) >= studyCourse.StartDate)
                {
                    studyCourse.Status = StudyCourseStatus.Ongoing;
                }
            }

            await _uow.CompleteAsync();
            _uow.CommitTran();
        }
    }
}