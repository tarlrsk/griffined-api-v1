using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class StudySubject
    {
        public int Id { get; set; }
        public int? SubjectId { get; set; }
        public int? StudyCourseId { get; set; }
        public double? Hour { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; }

        public virtual ICollection<StudySubjectMember> StudySubjectMember { get; set; }
        public virtual ICollection<StudyClass> StudyClasses { get; set; }

        public virtual ICollection<StudentAddingSubjectRequest> StudentAddingSubjectRequests { get; set; }
        public virtual ICollection<ClassCancellationRequest> ClassCancellationRequests { get; set; }
    }
}