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
        public int? courseId { get; set; }

        public string level { get; set; } = string.Empty;

        public virtual ICollection<NewCourseRequest> newCourseRequests { get; set; } = new List<NewCourseRequest>();

        [ForeignKey(nameof(courseId))]
        public virtual Course course { get; set; } = new Course();
    }
}