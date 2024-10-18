using Quartz;

namespace griffined_api.Jobs.UpdateStudentStatus
{
    public class UpdateStudentStatusJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(UpdateStudentStatusJob));
            options
                .AddJob<UpdateStudentStatusJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithCronSchedule("0 0 1 * * ?"));
            // .WithSimpleSchedule(schedule =>
            //     schedule.WithIntervalInMinutes(1).RepeatForever()));
        }
    }
}