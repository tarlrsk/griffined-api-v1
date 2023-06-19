using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Subject
    {
        public int id { get; set; }
        public string subject { get; set; } = string.Empty;
        public ICollection<Course> courses = new List<Course>();
    }
}