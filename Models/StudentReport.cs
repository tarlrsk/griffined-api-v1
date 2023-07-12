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
        public int Id { get; set; }
        public int? CourseMemberId { get; set; }
        public int? TeacherId { get; set; }

        public string Report { get; set; } = string.Empty;
        public virtual Progression Progression { get; set; }
        private DateTime _dateUploaded;
        public string DateUploaded { get; set; } = string.Empty;
        private DateTime _dateUpdated;
        public string DateUpdated { get; set; } = string.Empty;

        [ForeignKey(nameof(CourseMemberId))]
        public virtual CourseMember CourseMember { get; set; } = new CourseMember();

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; } = new Teacher();
    }
}