using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Level
    {
        public int id { get; set; }
        public ICollection<NewCourseRequest> newCourseRequests { get; set; } = new List<NewCourseRequest>();
    }
}