using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAddingRequest
    {
        public int id { get; set; }
        public int registrationRequestId { get; set; }
        public RegistrationRequest request { get; set; } = new RegistrationRequest();
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
    }
}