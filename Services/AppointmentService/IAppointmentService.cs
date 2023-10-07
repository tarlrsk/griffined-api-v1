using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.AppointentDtos;

namespace griffined_api.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<string>> AddNewAppointment(NewAppointmentRequestDto newAppointment);
        Task<ServiceResponse<List<AppointmentResponseDto>>> ListAllAppointments();
        Task<ServiceResponse<AppointmentDetailResponseDto>> GetAppointmentById(int appointmentId);
        Task<ServiceResponse<string>> UpdateApoointmentById(int appointmentId, UpdateAppointmentRequestDto updateAppointmentRequestDto);
    }
}