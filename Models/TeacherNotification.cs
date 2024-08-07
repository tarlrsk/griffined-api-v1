using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class TeacherNotification
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public int? StudyCourseId { get; set; }
        public int? AppointmentId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public virtual TeacherNotificationType Type { get; set; }
        public bool HasRead { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; }

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse? StudyCourse { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment? Appointment { get; set; }
    }
}