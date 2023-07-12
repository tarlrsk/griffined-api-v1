using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int? AppointmentId { get; set; }

        private DateTime _date;
        public string Date { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        public string FromTime { get; set; } = string.Empty;
        private TimeOnly _toTime;
        public string ToTime { get; set; } = string.Empty;

        public virtual ScheduleType Type { get; set; }
        public virtual StudyClass StudyClass { get; set; } = new StudyClass();

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment Appointment { get; set; } = new Appointment();
    }
}