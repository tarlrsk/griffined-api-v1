using Quartz;

namespace griffined_api.Jobs.BeginStudyCourse
{
    public class BeginStudyCourseJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(BeginStudyCourseJob));
            options
                .AddJob<BeginStudyCourseJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithCronSchedule("0 0 1 * * ?"));
        }
    }
}