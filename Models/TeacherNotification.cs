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
        public int id { get; set; }
        public int? teacherId { get; set; }
        public int? studyCourseId { get; set; }
        public int? appointmentId { get; set; }

        public DateTime dateCreated { get; set; } = DateTime.Now;
        public virtual TeacherNotificationType type { get; set; }
        public bool hasRead { get; set; }

        [ForeignKey(nameof(teacherId))]
        public virtual Teacher teacher { get; set; } = new Teacher();

        [ForeignKey(nameof(studyCourseId))]
        public virtual StudyCourse studyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(appointmentId))]
        public virtual Appointment appointment { get; set; } = new Appointment();
    }
}