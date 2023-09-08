using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.NotificationDto;

namespace griffined_api.Services.NotificationService
{
    public interface INotificationService
    {
        Task<ServiceResponse<List<StudentNotificationResponseDto>>> GetStudentNotifications();
    }
}