using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class AppointmentMember
    {
        public int id { get; set; }
        public int appointmentId { get; set; }
        public Appointment appointment { get; set; } = new Appointment();
        public int teacherId { get; set; }
        public Teacher teacher { get; set; } = new Teacher();
    }
}