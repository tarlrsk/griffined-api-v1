using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Teacher
    {
        public int id { get; set; }
        public string firebaseId { get; set; } = string.Empty;
        public string fName { get; set; } = string.Empty;
        public string lName { get; set; } = string.Empty;
        public string fullName { get { return fName + " " + lName; } }
        public string nickname { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string line { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public ICollection<WorkTime> workTimes { get; set; } = new List<WorkTime>();
        public ICollection<CourseMember> courseMembers { get; set; } = new List<CourseMember>();
        public ICollection<StudyClass> studyClasses { get; set; } = new List<StudyClass>();
        public ICollection<AppointmentMember> appointmentMembers { get; set; } = new List<AppointmentMember>();
        public ICollection<StudentReport> studentReports { get; set; } = new List<StudentReport>();
        public ICollection<TeacherNotification> teacherNotifications { get; set; } = new List<TeacherNotification>();
        public ICollection<CancellationRequest> cancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}