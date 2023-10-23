using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [Index(nameof(Date))]
    public class Schedule
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        public virtual ScheduleType Type { get; set; }
        public virtual StudyClass? StudyClass { get; set; }
        public virtual AppointmentSlot? AppointmentSlot { get; set; }
        public string? Room { get; set; }
    }
}