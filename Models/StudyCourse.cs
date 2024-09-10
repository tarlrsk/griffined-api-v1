using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    [Index(nameof(Status))]
    public class StudyCourse
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? LevelId { get; set; }

        public string Section { get; set; } = string.Empty;

        public double TotalHour { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual StudyCourseType StudyCourseType { get; set; }
        public virtual Method Method { get; set; }
        public virtual StudyCourseStatus Status { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }

        [ForeignKey(nameof(LevelId))]
        public virtual Level? Level { get; set; }
        public virtual NewCourseRequest? NewCourseRequest { get; set; }

        public virtual ICollection<StudentAddingRequest> StudentAddingRequests { get; set; }
        public virtual ICollection<StudySubject> StudySubjects { get; set; }
        public virtual ICollection<StudyClass> StudyClasses { get; set; }
        public virtual ICollection<StudyCourseHistory> StudyCourseHistories { get; set; }
        public virtual ICollection<StudentNotification> StudentNotifications { get; set; }
        public virtual ICollection<TeacherNotification> TeacherNotifications { get; set; }
        public virtual ICollection<StaffNotification> StaffNotifications { get; set; }
        public virtual ICollection<ClassCancellationRequest> ClassCancellationRequests { get; set; }
    }
}