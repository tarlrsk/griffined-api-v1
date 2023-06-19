using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAdditionalFile
    {
        public int id { get; set; }
        public string file { get; set; } = string.Empty;
        public int? studentId { get; set; }
        public Student? student { get; set; }
    }
}