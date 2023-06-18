using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class NewCourseSubjectRequest
    {
        public int id { get; set; }
        public ICollection<Subject> subjects { get; set; } = new List<Subject>();
        public ICollection<NewCourseRequest> newCourseRequests { get; set; } = new List<NewCourseRequest>();
        public int hour { get; set; }
    }
}