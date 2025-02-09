using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class StudyClass
    {
        public int Id { get; set; }
        public int? ScheduleId { get; set; }
        public int? StudyCourseId { get; set; }
        public int? StudySubjectId { get; set; }
        public int? TeacherId { get; set; }

        public int ClassNumber { get; set; }

        public virtual ClassStatus Status { get; set; }
        public bool IsMakeup { get; set; } = false;
        public bool IsFiftyPercent { get; set; }
        public bool IsHundredPercent { get; set; }
        public bool IsSubstitute { get; set; } = false;
        public bool HasAttemptedCancellation { get; set; } = false;

        [ForeignKey(nameof(ScheduleId))]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; }

        [ForeignKey(nameof(StudySubjectId))]
        public virtual StudySubject StudySubject { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; }

        public virtual ICollection<StudentAttendance> Attendances { get; set; }
        public virtual ICollection<ClassCancellationRequest> ClassCancellationRequests { get; set; }
        public virtual ICollection<StudyCourseHistory> StudyCourseHistories { get; set; }
        public virtual ICollection<TeacherShift> TeacherShifts { get; set; }
    }
}