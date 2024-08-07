using griffined_api.Jobs.UpdateClassStatus;
using griffined_api.Jobs.UpdateStudentStatus;
using Quartz;

namespace griffined_api.Jobs
{
    public static class QuartzDependencyInjecetion
    {
        public static void AddQuartzInfrastructure(this IServiceCollection services)
        {
            services.AddQuartz(options => { });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            services.ConfigureOptions<UpdateClassStatusJobSetup>();

            services.ConfigureOptions<UpdateStudentStatusJobSetup>();
        }
    }
}