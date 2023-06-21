using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class CourseMember
    {
        public int id { get; set; }
        public int? studentId { get; set; }
        public int? studySubjectId { get; set; }

        [ForeignKey(nameof(studentId))]
        public virtual Student student { get; set; } = new Student();

        [ForeignKey(nameof(studySubjectId))]
        public virtual StudySubject studySubject { get; set; } = new StudySubject();

        public virtual ICollection<StudentReport> studentReports { get; set; } = new List<StudentReport>();
    }
}