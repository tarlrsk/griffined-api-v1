using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace griffined_api.Jobs
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QuartzJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;
            var job = _serviceProvider.GetRequiredService(jobType) as IJob ?? throw new InvalidOperationException($"Failed to resolve job of type {jobType.FullName}");
            return job;
        }

        public void ReturnJob(IJob job)
        {
            // TODO when job is returned to factory
        }
    }

}