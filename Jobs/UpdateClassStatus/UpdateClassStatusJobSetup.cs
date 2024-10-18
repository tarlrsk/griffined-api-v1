using Quartz;

namespace griffined_api.Jobs.UpdateClassStatus
{
    public class UpdateClassStatusJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(UpdateClassStatusJob));
            options
                .AddJob<UpdateClassStatusJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithCronSchedule("0 0 1 * * ?"));
            // .WithSimpleSchedule(schedule =>
            //     schedule.WithIntervalInMinutes(1).RepeatForever()));
        }
    }
}