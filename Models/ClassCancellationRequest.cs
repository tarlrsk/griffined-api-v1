using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class ClassCancellationRequest
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? StudyCourseId { get; set; }
        public int? StudySubjectId { get; set; }
        public int? StudyClassId { get; set; }
        public int? TakenByEAId { get; set; }
        public string? RejectedReason { get; set; }

        public DateTime RequestedDate { get; set; }
        public virtual CancellationRole RequestedRole { get; set; }
        public virtual ClassCancellationRequestStatus Status { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student? Student { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher? Teacher { get; set; }

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(StudySubjectId))]
        public virtual StudySubject StudySubject { get; set; } = new StudySubject();

        [ForeignKey(nameof(StudyClassId))]
        public virtual StudyClass StudyClass { get; set; } = new StudyClass();

        public virtual ICollection<StaffNotification> StaffNotifications { get; set; }
    }
}