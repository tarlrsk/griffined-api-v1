using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class StudyAddingRequestDto
    {
        [Required]
        public required List<int> memberIds { get; set; }
        [Required]
        public PaymentType paymentType { get; set; }
        [Required]
        public List<int> courseIds{ get; set; } = new List<int>();
        [Required]
        public List<String> comments { get; set; } = new List<String>();
    }
}