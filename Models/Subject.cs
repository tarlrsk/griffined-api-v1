using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Subject
    {
        public int id { get; set; }
        public int courseId { get; set; }
        public Course course { get; set; } = new Course();
        public string subject { get; set; } = string.Empty;
        public ICollection<NewCourseSubjectRequest> newCourseSubjectRequests { get; set; } = new List<NewCourseSubjectRequest>();
        public ICollection<StudySubject> studySubjects { get; set; } = new List<StudySubject>();
    }
}