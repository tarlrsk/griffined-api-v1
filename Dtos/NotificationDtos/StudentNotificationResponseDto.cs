using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.NotificationDto
{
    public class StudentNotificationResponseDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int? StudyCourseId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
        public StudentNotificationType Type { get; set; }
        public bool HasRead { get; set; }
    }
}