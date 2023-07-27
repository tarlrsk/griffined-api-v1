using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyCourse
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? LevelId { get; set; }

        public string Section { get; set; } = string.Empty;

        public int TotalHour { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual StudyCourseType StudyCourseType { get; set; }
        public virtual Method Method { get; set; }
        public virtual CourseStatus Status { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; } = new Course();

        [ForeignKey(nameof(LevelId))]
        public virtual Level Level { get; set; } = new Level();

        public virtual ICollection<StudentAddingRequest> StudentAddingRequests { get; set; } = new List<StudentAddingRequest>();
        public virtual ICollection<StudySubject> StudySubjects { get; set; } = new List<StudySubject>();
        public virtual ICollection<StudyCourseHistory> StudyCourseHistories { get; set; } = new List<StudyCourseHistory>();
        public virtual ICollection<StudentNotification> StudentNotifications { get; set; } = new List<StudentNotification>();
        public virtual ICollection<TeacherNotification> TeacherNotifications { get; set; } = new List<TeacherNotification>();
        public virtual ICollection<StaffNotification> StaffNotifications { get; set; } = new List<StaffNotification>();
        public virtual ICollection<CancellationRequest> CancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}