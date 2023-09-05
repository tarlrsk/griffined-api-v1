using Quartz;
using System;
using System.Threading.Tasks;

namespace griffined_api.Jobs
{
    public class UpdateClassStatusJob : IJob
    {
        private readonly DataContext _context;
        public UpdateClassStatusJob(DataContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dbStudyClasses = await _context.StudyClasses.ToListAsync();

            foreach (var dbStudyClass in dbStudyClasses)
            {
                if (dbStudyClass.Status == ClassStatus.Checked)
                {
                    continue;
                }

                var classDate = dbStudyClass.Schedule.Date;
                var classEndTime = classDate.Add(dbStudyClass.Schedule.ToTime);

                if (DateTime.Now > classEndTime)
                {
                    dbStudyClass.Status = ClassStatus.Unchecked;

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}