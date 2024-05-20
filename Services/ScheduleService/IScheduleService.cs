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
        Task<ServiceResponse<List<TodayMobileResponseDto>>> GetMobileTodayClass(string date);
        Task<ServiceResponse<string>> UpdateStudyClassRoomByScheduleIds(List<UpdateRoomRequestDto> requestDto);

        /// <summary>
        /// Generate available schedule by checking the given parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ServiceResponse<IEnumerable<AvailableAppointmentScheduleDTO>> GenerateAvailableSchedule(CheckAvailableAppointmentScheduleDTO request);
    }
}