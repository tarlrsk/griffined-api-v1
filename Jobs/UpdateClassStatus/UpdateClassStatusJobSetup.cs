using Microsoft.Extensions.Options;
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
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(10).RepeatForever()));
        }
    }
}