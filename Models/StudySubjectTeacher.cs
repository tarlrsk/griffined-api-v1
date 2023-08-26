using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudySubjectTeacher
    {
        public int StudySubjectTeacherId { get; set; }
        public int StudySubjectMemberId { get; set; }
        public int TeacherId { get; set; }

        public DateTime CourseJoinedDate { get; set; }

        [ForeignKey(nameof(StudySubjectMemberId))]
        public virtual StudySubjectMember? StudySubjectMember { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher? Teacher { get; set; }
    }
}