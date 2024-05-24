using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [Table("Schedules")]
    [Index(nameof(Date))]
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan FromTime { get; set; }

        public TimeSpan ToTime { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public virtual ScheduleType Type { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public virtual DailyCalendarType? CalendarType { get; set; }

        public virtual StudyClass? StudyClass { get; set; }

        public virtual AppointmentSlot? AppointmentSlot { get; set; }

        public string? Room { get; set; }
    }
}