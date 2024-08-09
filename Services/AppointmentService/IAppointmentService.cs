using griffined_api.Dtos.AppointentDtos;

namespace griffined_api.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<List<AppointmentResponseDto>>> ListAllAppointments();
        Task<ServiceResponse<AppointmentDetailResponseDto>> GetAppointmentById(int appointmentId);
        Task<ServiceResponse<string>> UpdateApoointmentById(int appointmentId, UpdateAppointmentRequestDto updateAppointmentRequestDto);

        Appointment CreateAppointment(CreateAppointmentDTO request);
        void CreateAppointmentMember(IEnumerable<int> teacherIds, Appointment appointment);
        void CreateAppointmentNotification(IEnumerable<int> teacherIds, Appointment appointment);
        IEnumerable<Schedule> CreateAppointmentSchedule(CreateAppointmentDTO request, Appointment appointment);
        void CreateAppointmentSlot(IEnumerable<Schedule> schedules, Appointment appointment);
        void DeleteTeacherAppointmentNotification(int id);

        void DeleteAppointment(int id);
        void DeleteAppointmentMember(int id);
        void DeleteAppointmentSchedule(int id);
        void DeleteAppointmentSlot(int id);
    }
}