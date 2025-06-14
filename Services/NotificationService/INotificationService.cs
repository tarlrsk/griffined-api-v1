using griffined_api.Dtos.NotificationDto;

namespace griffined_api.Services.NotificationService
{
    public interface INotificationService
    {
        Task<ServiceResponse<List<StudentNotificationResponseDto>>> GetStudentNotifications();
        Task<ServiceResponse<List<TeacherNotificationResponseDto>>> GetTeacherNotifications();
        Task<ServiceResponse<List<StaffNotificationResponseDto>>> GetStaffNotifications();
        Task<ServiceResponse<string>> MarkAsRead(int notificationId);
        Task<ServiceResponse<string>> MarkAllAsRead();
    }
}