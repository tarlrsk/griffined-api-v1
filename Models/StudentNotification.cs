using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentNotification
    {
        public int id { get; set; }
        public DateTime dateCreated { get; set; } = DateTime.Now;
        public StudentNotificationType type { get; set; }
        public bool hasRead { get; set; }
        public int studentId { get; set; }
        public Student student { get; set; } = new Student();
        public int studyCourseId { get; set; }
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
    }
}