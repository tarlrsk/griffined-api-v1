using Quartz;

namespace griffined_api.Jobs.UpdateClassStatus
{
    [DisallowConcurrentExecution]
    public class UpdateClassStatusJob : IJob
    {
        private readonly DataContext _context;
        private readonly ILogger<UpdateClassStatusJob> _logger;
        public UpdateClassStatusJob(DataContext context, ILogger<UpdateClassStatusJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dbStudyClasses = await _context.StudyClasses.ToListAsync();

            foreach (var dbStudyClass in dbStudyClasses)
            {
                var classEndTime = dbStudyClass.Schedule.Date.Add(dbStudyClass.Schedule.ToTime);

                if (dbStudyClass.Status == ClassStatus.Checked)
                {
                    _logger.LogInformation($"No changes at {DateTime.Now}");
                    continue;
                }

                if (DateTime.Now > classEndTime)
                {
                    dbStudyClass.Status = ClassStatus.Unchecked;
                    _logger.LogInformation($"Class Status changed to Unchecked at {DateTime.Now}");
                    await _context.SaveChangesAsync();
                }
            }

            await Task.CompletedTask;
        }
    }
}