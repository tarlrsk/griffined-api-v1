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
        public string Date { get { return _date.ToString("dd-MMMM-yyyy HH:mm:ss");} set {_date = DateTime.Parse(value);} }
        private TimeOnly _fromTime;
        public string FromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        public string ToTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }

        public virtual ScheduleType Type { get; set; }
        public virtual StudyClass? StudyClass { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment? Appointment { get; set; }
    }
}