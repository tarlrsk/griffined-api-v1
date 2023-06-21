using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Payment
    {
        public int id { get; set; }
        public int? registrationRequestId { get; set; }

        public string file { get; set; } = string.Empty;

        [ForeignKey(nameof(registrationRequestId))]
        public virtual RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();
    }
}