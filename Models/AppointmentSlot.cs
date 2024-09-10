using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class AppointmentSlot
    {
        public int Id { get; set; }
        public int? AppointmentId { get; set; }
        public int? ScheduleId { get; set; }
        public AppointmentSlotStatus AppointmentSlotStatus { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment Appointment { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public virtual Schedule Schedule { get; set; }
        public virtual ICollection<AppointmentHistory> AppointmentHistories { get; set; }
    }
}