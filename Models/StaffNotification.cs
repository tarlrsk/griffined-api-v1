using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StaffNotification
    {
        public int id { get; set; }
        public DateTime dateCreated { get; set; } = DateTime.Now;
        public StaffNotificationType type { get; set; }
        public bool hasRead { get; set; }
        public int staffId { get; set; }
        public Staff staff { get; set; } = new Staff();
        public int studyCourseId { get; set; }
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
    }
}