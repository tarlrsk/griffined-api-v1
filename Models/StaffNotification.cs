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
        public _StaffNotificationTypeEnum type { get; set; }
        public bool hasRead { get; set; }
        public ICollection<Staff> staff { get; set; } = new List<Staff>();
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
    }
}