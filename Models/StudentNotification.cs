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
        public _StudentNotificationTypeEnum type { get; set; }
        public bool hasRead { get; set; }
        public ICollection<Student> students { get; set; } = new List<Student>();
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
    }
}