namespace griffined_api.Models
{
    public class Staff
    {
        public int Id { get; set; }

        public string? FirebaseId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        public string Nickname { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<AppointmentHistory> AppointmentHistories { get; set; }
        public virtual ICollection<StaffNotification> StaffNotifications { get; set; }
        public virtual ICollection<StudyCourseHistory> StudyCourseHistories { get; set; }
        public virtual ICollection<RegistrationRequestComment> RegistrationRequestComments { get; set; }
    }
}