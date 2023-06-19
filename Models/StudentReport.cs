using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentReport
    {
        public int id { get; set; }
        public string report { get; set; } = string.Empty;
        public Progression progression { get; set; }
        private DateTime _dateUploaded;
        public string dateUploaded { get; set; } = string.Empty;
        private DateTime _dateUpdated;
        public string dateUpdated { get; set; } = string.Empty;
        public int courseMemberId { get; set; }
        public CourseMember courseMember { get; set; } = new CourseMember();
        public int teacherId { get; set; }
        public Teacher teacher { get; set; } = new Teacher();
    }
}