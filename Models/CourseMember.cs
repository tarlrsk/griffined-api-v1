using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class CourseMember
    {
        public int id { get; set; }
        public ICollection<StudentReport> studentReports { get; set; } = new List<StudentReport>();
        public int studySubjectId { get; set; }
        public StudySubject studySubject { get; set; } = new StudySubject();
        public int studentId { get; set; }
        public Student student { get; set; } = new Student();
    }
}