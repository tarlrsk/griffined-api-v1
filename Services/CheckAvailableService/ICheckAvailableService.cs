using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.CheckAvailableService
{
    public interface ICheckAvailableService
    {
        Task<ServiceResponse<CheckScheduleResultResponseDto>> GetAvailableSchedule(RequestedScheduleRequestDto requestedSchedule);
        Task<ServiceResponse<List<AvailableTeacherResponseDto>>> GetAvailableTeacherForAppointment(List<LocalAppointmentRequestDto> appointmentRequestDtos);
        Task<ServiceResponse<CheckAppointmentConflictResponseDto>> CheckAppointmentConflict(CheckAppointmentConflictRequestDto requestDto);
    }
}