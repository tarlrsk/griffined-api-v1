using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyCourse
    {
        public int id { get; set; }
        public int courseId { get; set; }
        public Course course { get; set; } = new Course();
        public CourseStatus status { get; set; }
        public ICollection<StudySubject> studySubjects { get; set; } = new List<StudySubject>();
        public ICollection<PreferredDay> preferredDays { get; set; } = new List<PreferredDay>();
        public ICollection<StudyCourseHistory> studyCourseHistories { get; set; } = new List<StudyCourseHistory>();
        public ICollection<StudentNotification> studentNotifications { get; set; } = new List<StudentNotification>();
        public ICollection<TeacherNotification> teacherNotifications { get; set; } = new List<TeacherNotification>();
        public ICollection<CancellationRequest> cancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}