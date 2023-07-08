using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<ServiceResponse<String>> AddGroupSchedule();
    }
}