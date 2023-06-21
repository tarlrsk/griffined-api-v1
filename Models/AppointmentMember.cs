using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class AppointmentMember
    {
        public int id { get; set; }
        public int? teacherId { get; set; }
        public int? appointmentId { get; set; }

        [ForeignKey(nameof(appointmentId))]
        public virtual Appointment appointment { get; set; } = new Appointment();

        [ForeignKey(nameof(teacherId))]
        public virtual Teacher teacher { get; set; } = new Teacher();
    }
}