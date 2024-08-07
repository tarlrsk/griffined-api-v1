using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class AppointmentMember
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public int? AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment Appointment { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; }
    }
}