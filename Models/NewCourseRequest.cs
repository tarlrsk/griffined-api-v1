using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class NewCourseRequest
    {
        public int id { get; set; }
        public int registrationRequestId { get; set; }
        public RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();
        public int courseId { get; set; }
        public Course course { get; set; } = new Course();
        public int levelId { get; set; }
        public Level level { get; set; } = new Level();
        public Method method { get; set; }
        public int totalHours { get; set; }

    }
}