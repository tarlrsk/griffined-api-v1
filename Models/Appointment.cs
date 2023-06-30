using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Appointment
    {
        public int id { get; set; }

        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;

        public virtual AppointmentType appointmentType { get; set; }
        public virtual AppointmentStatus appointmentStatus { get; set; }

        public virtual ICollection<TeacherNotification> teacherNotifications { get; set; } = new List<TeacherNotification>();
        public virtual ICollection<AppointmentMember> appointmentMembers { get; set; } = new List<AppointmentMember>();
        public virtual ICollection<Schedule> schedules { get; set; } = new List<Schedule>();
    }
}