using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyCourseHistory
    {
        public int id { get; set; }
        public string description { get; set; } = string.Empty;
        private DateTime _dateUpdated;
        public string dateUpdated { get; set; } = string.Empty;
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
        public ICollection<Staff> byStaff { get; set; } = new List<Staff>();
    }
}