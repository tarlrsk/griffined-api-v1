using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Course
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string course { get; set; } = string.Empty;
        [Required]
        public string subject { get; set; } = string.Empty;
        [Required]
        public string level { get; set; } = string.Empty;
    }
}