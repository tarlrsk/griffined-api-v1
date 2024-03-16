using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.CheckAvailableService
{
    public interface ICheckAvailableService
    {
        Task<ServiceResponse<CheckScheduleResultResponseDto>> GetAvailableSchedule(RequestedScheduleRequestDto request);
        Task<ServiceResponse<List<AvailableTeacherResponseDto>>> GetAvailableTeacherForAppointment(int? appointmentId, List<LocalAppointmentRequestDto> request);
        Task<ServiceResponse<CheckAppointmentConflictResponseDto>> CheckAppointmentConflict(CheckAppointmentConflictRequestDto request);
        Task<ServiceResponse<StudentAddingConflictResponseDto>> CheckStudentAddingConflict(StudentAddingConflictRequestDto request);
    }
}