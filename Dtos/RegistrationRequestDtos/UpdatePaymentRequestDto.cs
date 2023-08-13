using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class UpdatePaymentRequestDto
    {
        public PaymentStatus? PaymentStatus { get; set; }
        public List<string> FilesToDelete { get; set; } = new List<string>();
        public List<IFormFile> FilesToUpload { get; set; } = new List<IFormFile>();
    }
}