using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RegistrationRequestMember
    {
        public int id { get; set; }
        public int? studentId { get; set; }
        public int? registrationRequestId { get; set; }

        [ForeignKey(nameof(studentId))]
        public virtual Student student { get; set; } = new Student();

        [ForeignKey(nameof(registrationRequestId))]
        public virtual RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();
    }
}