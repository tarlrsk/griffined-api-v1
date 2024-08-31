using Google.Api;
using griffined_api.Dtos.ScheduleDtos;

namespace griffined_api.Services.ScheduleService
{
    public interface IScheduleService
    {
        ServiceResponse<List<DailtyCalendarDTO>> GetDailyCalendarForStaff(string date);
        Task<ServiceResponse<List<TodayMobileResponseDto>>> GetMobileTodayClass(string date);
        Task<ServiceResponse<string>> UpdateStudyClassRoomByScheduleIds(List<UpdateRoomRequestDto> requestDto);

        /// <summary>
        /// Generate available appointment schedule by checking the given parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ServiceResponse<AvailableAppointmentDTO> GenerateAvailableAppointmentSchedule(CheckAvailableAppointmentScheduleDTO request);

        /// <summary>
        /// Generate available class schedule by checking the given parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ServiceResponse<AvailableClassScheduleDTO> GenerateAvailableClassSchedule(CheckAvailableClassScheduleDTO request);

        /// <summary>
        /// Check if the teacher is availabled to be added to existing appointment.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ServiceResponse<AvailableDTO> CheckAvailableTeacherAppointment(int appointmentId, CheckAvailableTeacherAppointmentDTO request);
    }
}