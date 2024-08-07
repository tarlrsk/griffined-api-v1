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
        public int Id { get; set; }
        public int? CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }

        public string subject { get; set; } = string.Empty;

        public virtual ICollection<NewCourseSubjectRequest> NewCourseSubjectRequests { get; set; }
        public virtual ICollection<StudySubject> StudySubjects { get; set; }
    }
}