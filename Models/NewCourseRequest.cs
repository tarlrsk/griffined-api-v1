using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class NewCourseRequest
    {
        public int id { get; set; }
        public ICollection<RegistrationRequest> registrationRequests { get; set; } = new List<RegistrationRequest>();
        public ICollection<Course> courses { get; set; } = new List<Course>();
        public ICollection<Level> levels { get; set; } = new List<Level>();
        public _MethodEnum method { get; set; }
        public int totalHours { get; set; }

    }
}