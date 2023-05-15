using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAdditionalFiles
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string file { get; set; } = string.Empty;
        public int? studentId { get; set; }
        public Student? student { get; set; }
    }
}