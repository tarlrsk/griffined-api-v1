using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class ExamDate
    {
        public int id { get; set; }
        private DateTime _examDate;
        public string examDate { get { return _examDate.ToString("dd/MM/yyyy HH:mm:ss"); } set { _examDate = DateTime.Parse(value); } }
        public int? studentId { get; set; }
        public Student? student { get; set; }
        public int? privateCourseId { get; set; }
        public PrivateCourse? privateCourse { get; set; }
    }
}