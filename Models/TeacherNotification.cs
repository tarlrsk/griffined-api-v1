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
        public _TeacherNotificationTypeEnum type { get; set; }
        public bool hasRead { get; set; }
        public ICollection<Teacher> teachers { get; set; } = new List<Teacher>();
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
        public ICollection<Appointment> appointments { get; set; } = new List<Appointment>();
    }
}