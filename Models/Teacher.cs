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
        public int Id { get; set; }

        public string? FirebaseId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        public string Nickname { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }

        public virtual ICollection<WorkTime> WorkTimes { get; set; } = new List<WorkTime>();
        public virtual ICollection<StudyClass> StudyClasses { get; set; } = new List<StudyClass>();
        public virtual ICollection<AppointmentHistory> AppointmentHistories { get; set; } = new List<AppointmentHistory>();
        public virtual ICollection<AppointmentMember> AppointmentMembers { get; set; } = new List<AppointmentMember>();
        public virtual ICollection<StudentReport> StudentReports { get; set; } = new List<StudentReport>();
        public virtual ICollection<TeacherNotification> TeacherNotifications { get; set; } = new List<TeacherNotification>();
        public virtual ICollection<ClassCancellationRequest> ClassCancellationRequests { get; set; } = new List<ClassCancellationRequest>();
        public virtual ICollection<TeacherShift> TeacherShifts { get; set; } = new List<TeacherShift>();
    }
}