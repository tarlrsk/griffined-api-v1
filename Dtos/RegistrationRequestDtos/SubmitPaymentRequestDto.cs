using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class SubmitPaymentRequestDto
    {
        [Required]
        public PaymentType PaymentType { get; set; }
        public List<string> FilesToDelete { get; set; } = new List<string>();
    }
}