using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class PaymentFile
    {
        public int id { get; set; }
        public string file { get; set; } = string.Empty;
        public int registrationRequestId { get; set; }
        public RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();
    }
}