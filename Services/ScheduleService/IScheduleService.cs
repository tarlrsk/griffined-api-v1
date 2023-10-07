using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.ScheduleDtos;

namespace griffined_api.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<ServiceResponse<List<DailyCalendarResponseDto>>> GetDailyCalendarForStaff(string date);
    }
}