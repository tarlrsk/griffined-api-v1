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
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public int? AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment Appointment { get; set; } = new Appointment();

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; } = new Teacher();
    }
}