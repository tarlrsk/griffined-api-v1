using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class WorkTime
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }

        public string Day { get; set; } = string.Empty;
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public Teacher? Teacher { get; set; }
    }
}