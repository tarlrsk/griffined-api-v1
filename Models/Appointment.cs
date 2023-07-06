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
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public virtual AppointmentType AppointmentType { get; set; }
        public virtual AppointmentStatus AppointmentStatus { get; set; }

        public virtual ICollection<TeacherNotification> TeacherNotifications { get; set; } = new List<TeacherNotification>();
        public virtual ICollection<AppointmentMember> AppointmentMembers { get; set; } = new List<AppointmentMember>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}