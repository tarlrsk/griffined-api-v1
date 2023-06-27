using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Course
    {
        public int id { get; set; }

        public string course { get; set; } = string.Empty;

        public virtual ICollection<Subject> subjects { get; set; } = new List<Subject>();
        public virtual ICollection<Level> levels { get; set; } = new List<Level>();
    }
}