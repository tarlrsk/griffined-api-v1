using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyCourseHistory
    {
        public int id { get; set; }
        public int? studyCourseId { get; set; }
        public int? staffId { get; set; }

        public string description { get; set; } = string.Empty;
        private DateTime _dateUpdated;
        public string dateUpdated { get; set; } = string.Empty;

        [ForeignKey(nameof(studyCourseId))]
        public virtual StudyCourse studyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(staffId))]
        public virtual Staff byStaff { get; set; } = new Staff();
    }
}