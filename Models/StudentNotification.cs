using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class StudentNotification
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? StudyCourseId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public virtual StudentNotificationType Type { get; set; }
        public bool HasRead { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse? StudyCourse { get; set; }
    }
}