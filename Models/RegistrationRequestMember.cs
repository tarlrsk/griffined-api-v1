using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RegistrationRequestMember
    {
        public int id { get; set; }
        public ICollection<Student> students { get; set; } = new List<Student>();
        public ICollection<RegistrationRequest> registrationRequests { get; set; } = new List<RegistrationRequest>();
    }
}