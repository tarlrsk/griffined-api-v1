namespace griffined_api.Models
{
    public class RegistrationRequest
    {
        public int Id { get; set; }
        public int? CreatedByStaffId { get; set; }
        public int? PaymentByStaffId { get; set; }
        public int? ScheduledByStaffId { get; set; }
        public int? ReviewedByStaffId { get; set; }
        public int? CancelledBy { get; set; }
        public int? TakenByEAId { get; set; }

        public string Section { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public RegistrationRequestType Type { get; set; }
        public bool PaymentError { get; set; }
        public bool ScheduleError { get; set; }
        public bool NewCourseDetailError { get; set; }
        public bool HasSchedule { get; set; }
        public virtual RegistrationStatus RegistrationStatus { get; set; }

        public virtual PaymentType? PaymentType { get; set; }
        public virtual PaymentStatus? PaymentStatus { get; set; }

        public virtual ICollection<StudentAddingRequest> StudentAddingRequest { get; set; }
        public virtual ICollection<NewCourseRequest> NewCourseRequests { get; set; }
        public virtual ICollection<RegistrationRequestPaymentFile> RegistrationRequestPaymentFiles { get; set; }
        public virtual ICollection<RegistrationRequestMember> RegistrationRequestMembers { get; set; }
        public virtual ICollection<NewCoursePreferredDayRequest> NewCoursePreferredDayRequests { get; set; }
        public virtual ICollection<RegistrationRequestComment> RegistrationRequestComments { get; set; }
        public virtual ICollection<StaffNotification> StaffNotifications { get; set; }
    }
}