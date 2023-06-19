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
        public int studyCourseId { get; set; }
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
        public int byStaffId { get; set; }
        public Staff byStaff { get; set; } = new Staff();
    }
}