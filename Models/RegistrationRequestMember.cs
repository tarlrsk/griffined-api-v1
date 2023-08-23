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
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? RegistrationRequestId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = new Student();

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest RegistrationRequest { get; set; } = new RegistrationRequest();
    }
}