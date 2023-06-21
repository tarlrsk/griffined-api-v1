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
        public int id { get; set; }
        public int? courseId { get; set; }
        public virtual CourseStatus status { get; set; }

        [ForeignKey(nameof(courseId))]
        public virtual Course course { get; set; } = new Course();

        public virtual ICollection<StudySubject> studySubjects { get; set; } = new List<StudySubject>();
        public virtual ICollection<PreferredDay> preferredDays { get; set; } = new List<PreferredDay>();
        public virtual ICollection<StudyCourseHistory> studyCourseHistories { get; set; } = new List<StudyCourseHistory>();
        public virtual ICollection<StudentNotification> studentNotifications { get; set; } = new List<StudentNotification>();
        public virtual ICollection<TeacherNotification> teacherNotifications { get; set; } = new List<TeacherNotification>();
        public virtual ICollection<CancellationRequest> cancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}