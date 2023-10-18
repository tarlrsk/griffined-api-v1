using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudySubjectMember
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int? StudySubjectId { get; set; }

        public StudySubjectMemberStatus Status { get; set; }
        public DateTime CourseJoinedDate { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = new Student();

        [ForeignKey(nameof(StudySubjectId))]
        public virtual StudySubject StudySubject { get; set; } = new StudySubject();

        public virtual ICollection<StudentReport> StudentReports { get; set; } = new List<StudentReport>();
    }
}