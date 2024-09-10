using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreatedByStaffId { get; set; }

        public virtual AppointmentType AppointmentType { get; set; }
        public virtual AppointmentStatus AppointmentStatus { get; set; }


        [ForeignKey(nameof(CreatedByStaffId))]
        public virtual Staff Staff { get; set; }

        public virtual ICollection<TeacherNotification> TeacherNotifications { get; set; }
        public virtual ICollection<AppointmentHistory> AppointmentHistories { get; set; }
        public virtual ICollection<AppointmentMember> AppointmentMembers { get; set; }
        public virtual ICollection<AppointmentSlot> AppointmentSlots { get; set; }
    }
}