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
        public int? StudySubjectMemberId { get; set; }
        public int? TeacherId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }

        public virtual Progression Progression { get; set; }

        [ForeignKey(nameof(StudySubjectMemberId))]
        public virtual StudySubjectMember StudySubjectMember { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; }
    }
}