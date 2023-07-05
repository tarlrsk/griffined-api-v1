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
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }

        public string File { get; set; } = string.Empty;

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest RegistrationRequest { get; set; } = new RegistrationRequest();
    }
}