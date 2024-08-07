using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public bool IsPartTime { get; set; } = false;
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }

        public virtual ICollection<Manday> Mandays { get; set; }
        public virtual ICollection<StudyClass> StudyClasses { get; set; }
        public virtual ICollection<AppointmentHistory> AppointmentHistories { get; set; }
        public virtual ICollection<AppointmentMember> AppointmentMembers { get; set; }
        public virtual ICollection<StudentReport> StudentReports { get; set; }
        public virtual ICollection<TeacherNotification> TeacherNotifications { get; set; }
        public virtual ICollection<ClassCancellationRequest> ClassCancellationRequests { get; set; }
        public virtual ICollection<TeacherShift> TeacherShifts { get; set; }
    }
}