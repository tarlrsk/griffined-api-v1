using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual Student Student { get; set; }

        [ForeignKey(nameof(StudySubjectId))]
        public virtual StudySubject StudySubject { get; set; }

        public virtual ICollection<StudentReport> StudentReports { get; set; }
    }
}