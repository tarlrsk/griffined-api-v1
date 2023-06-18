using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class AppointmentMember
    {
        public int id { get; set; }
        public ICollection<Appointment> appointments { get; set; } = new List<Appointment>();
        public ICollection<Teacher> teachers { get; set; } = new List<Teacher>();
    }
}