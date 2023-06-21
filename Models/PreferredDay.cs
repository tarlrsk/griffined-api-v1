using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class PreferredDay
    {
        public int id { get; set; }
        public int? studyCourseId { get; set; }

        public string day { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        public string fromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        public string toTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }

        [ForeignKey(nameof(studyCourseId))]
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
    }
}