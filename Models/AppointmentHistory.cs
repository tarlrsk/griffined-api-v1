using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class AppointmentHistory
    {
        public int Id { get; set; }
        public int? AppointmentId { get; set; }
        public int? StaffId { get; set; }
        public int? TeacherId { get; set; }
        public int? AppointmentSlotId { get; set; }

        public AppointmentHistoryMethod Method { get; set; }
        public AppointmentHistoryType Type { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment Appointment { get; set; } = new Appointment();

        [ForeignKey(nameof(AppointmentSlotId))]
        public virtual AppointmentSlot? AppointmentSlot { get; set; } 

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher? Teacher { get; set; }

        [ForeignKey(nameof(StaffId))]
        public virtual Staff? Staff { get; set; }

    }
}