using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Course
    {
        public int id { get; set; }
        public string course { get; set; } = string.Empty;
        public ICollection<Subject>? subjects { get; set; }
    }
}