using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class TeacherNotification
    {
        public int id { get; set; }
        public DateTime dateCreated { get; set; } = DateTime.Now;
        public TeacherNotificationType type { get; set; }
        public bool hasRead { get; set; }
        public int teacherId { get; set; }
        public Teacher teacher { get; set; } = new Teacher();
        public int studyCourseId { get; set; }
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
        public int appointmentId { get; set; }
        public Appointment appointment { get; set; } = new Appointment();
    }
}