using Quartz;

namespace griffined_api.Jobs.UpdateClassStatus
{
    [DisallowConcurrentExecution]
    public class UpdateClassStatusJob : IJob
    {
        private readonly ILogger<UpdateClassStatusJob> _logger;
        public UpdateClassStatusJob(ILogger<UpdateClassStatusJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Job executed at: {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}