using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Appointment
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public AppointmentType appointmentType { get; set; }
        public AppointmentStatus appointmentStatus { get; set; }
        public ICollection<AppointmentMember> appointmentMembers { get; set; } = new List<AppointmentMember>();
        public ICollection<Schedule> schedules { get; set; } = new List<Schedule>();
    }
}