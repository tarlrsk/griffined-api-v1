using Quartz;

namespace griffined_api.Jobs.UpdateClassStatus
{
    [DisallowConcurrentExecution]
    public class UpdateClassStatusJob : IJob
    {
        private readonly DataContext _context;
        public UpdateClassStatusJob(DataContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dbStudyClasses = await _context.StudyClasses
                                .Include(sc => sc.Schedule)
                                .ToListAsync();

            foreach (var dbStudyClass in dbStudyClasses)
            {
                var classEndTime = dbStudyClass.Schedule.Date.Add(dbStudyClass.Schedule.ToTime);

                if (dbStudyClass.Status == ClassStatus.NONE)
                {
                    if (DateTime.Now >= classEndTime)
                    {
                        dbStudyClass.Status = ClassStatus.UNCHECKED;

                        await _context.SaveChangesAsync();
                    }
                }
            }

            await Task.CompletedTask;
        }
    }
}