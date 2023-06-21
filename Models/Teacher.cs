using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public virtual ICollection<WorkTime> workTimes { get; set; } = new List<WorkTime>();
        public virtual ICollection<StudyClass> studyClasses { get; set; } = new List<StudyClass>();
        public virtual ICollection<AppointmentMember> appointmentMembers { get; set; } = new List<AppointmentMember>();
        public virtual ICollection<StudentReport> studentReports { get; set; } = new List<StudentReport>();
        public virtual ICollection<TeacherNotification> teacherNotifications { get; set; } = new List<TeacherNotification>();
        public virtual ICollection<CancellationRequest> cancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}