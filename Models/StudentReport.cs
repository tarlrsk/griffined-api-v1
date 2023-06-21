using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentReport
    {
        public int id { get; set; }
        public int? courseMemberId { get; set; }
        public int? teacherId { get; set; }

        public string report { get; set; } = string.Empty;
        public virtual Progression progression { get; set; }
        private DateTime _dateUploaded;
        public string dateUploaded { get; set; } = string.Empty;
        private DateTime _dateUpdated;
        public string dateUpdated { get; set; } = string.Empty;

        [ForeignKey(nameof(courseMemberId))]
        public virtual CourseMember courseMember { get; set; } = new CourseMember();

        [ForeignKey(nameof(teacherId))]
        public virtual Teacher teacher { get; set; } = new Teacher();
    }
}