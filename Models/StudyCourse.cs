using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyCourse
    {
        public int id { get; set; }
        public ICollection<Course> courses { get; set; } = new List<Course>();
        public _CourseStatusEnum status { get; set; }
    }
}