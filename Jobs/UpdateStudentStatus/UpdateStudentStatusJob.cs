using Quartz;

namespace griffined_api.Jobs.UpdateStudentStatus
{
    [DisallowConcurrentExecution]
    public class UpdateStudentStatusJob : IJob
    {
        private readonly DataContext _context;
        public UpdateStudentStatusJob(DataContext context)
        {
            _context = context;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var dbStudents = await _context.Students.ToListAsync();

            foreach (var dbStudent in dbStudents)
            {
                if (DateTime.Now > dbStudent.ExpiryDate)
                {
                    dbStudent.Status = StudentStatus.Inactive;
                }
            }

            await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }
    }
}