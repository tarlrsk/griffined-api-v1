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
        public _AppointmentTypeEnum appointmentType { get; set; }
        public _AppointmentStatusEnum appointmentStatus { get; set; }
    }
}