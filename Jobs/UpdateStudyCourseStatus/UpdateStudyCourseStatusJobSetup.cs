using Quartz;

namespace griffined_api.Jobs.UpdateStudyCourseStatus
{
    public class UpdateStudyCourseStatusJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(UpdateStudyCourseStatusJob));
            options
                .AddJob<UpdateStudyCourseStatusJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithCronSchedule("0 0 1 * * ?"));
        }
    }
}