namespace griffined_api.Dtos.NotificationDto
{
    public class TeacherNotificationResponseDto
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int? StudyCourseId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
        public TeacherNotificationType Type { get; set; }
        public bool HasRead { get; set; }
    }
}