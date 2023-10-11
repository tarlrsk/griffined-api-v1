using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.NotificationDto
{
    public class StaffNotificationResponseDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int? StudyCourseId { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? CancellationRequestId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
        public StaffNotificationType Type { get; set; }
        public bool HasRead { get; set; }
    }
}