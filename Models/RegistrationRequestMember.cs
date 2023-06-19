using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RegistrationRequestMember
    {
        public int id { get; set; }
        public int studentId { get; set; }
        public Student student { get; set; } = new Student();
        public int registrationRequestId { get; set; }
        public RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();
    }
}