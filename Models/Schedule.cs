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
        public int id { get; set; }
        public int? appointmentId { get; set; }

        private DateTime _date;
        public string date { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        public string fromTime { get; set; } = string.Empty;
        private TimeOnly _toTime;
        public string toTime { get; set; } = string.Empty;

        public virtual ScheduleType type { get; set; }
        public virtual StudyClass studyClass { get; set; } = new StudyClass();

        [ForeignKey(nameof(appointmentId))]
        public virtual Appointment appointment { get; set; } = new Appointment();
    }
}