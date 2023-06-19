using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Schedule
    {
        public int id { get; set; }
        private DateTime _date;
        public string date { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        public string fromTime { get; set; } = string.Empty;
        private TimeOnly _toTime;
        public string toTime { get; set; } = string.Empty;
        public ScheduleType type { get; set; }
        public StudyClass studyClass { get; set; } = new StudyClass();
        public int appointmentId { get; set; }
        public Appointment appointment { get; set; } = new Appointment();
    }
}