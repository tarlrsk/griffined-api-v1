using griffined_api.Jobs.BeginStudyCourse;
using griffined_api.Jobs.UpdateClassStatus;
using griffined_api.Jobs.UpdateStudentStatus;
using griffined_api.Jobs.UpdateStudyCourseStatus;
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

            services.ConfigureOptions<UpdateStudyCourseStatusJobSetup>();

            services.ConfigureOptions<BeginStudyCourseJob>();
        }
    }
}