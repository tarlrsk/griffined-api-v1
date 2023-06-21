using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Subject
    {
        public int id { get; set; }
        public int? courseId { get; set; }

        [ForeignKey(nameof(courseId))]
        public virtual Course course { get; set; } = new Course();

        public string subject { get; set; } = string.Empty;

        public virtual ICollection<NewCourseSubjectRequest> newCourseSubjectRequests { get; set; } = new List<NewCourseSubjectRequest>();
        public virtual ICollection<StudySubject> studySubjects { get; set; } = new List<StudySubject>();
    }
}