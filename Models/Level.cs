using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Level
    {
        public int id { get; set; }

        public virtual ICollection<NewCourseRequest> newCourseRequests { get; set; } = new List<NewCourseRequest>();
    }
}