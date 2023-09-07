using griffined_api.Jobs.UpdateClassStatus;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}