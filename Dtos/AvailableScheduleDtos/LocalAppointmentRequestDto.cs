using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class LocalAppointmentRequestDto
    {
        [Required]
        public string Date { get; set; } = string.Empty;
        [Required]
        public string FromTime { get; set; } = string.Empty;
        [Required]
        public string ToTime { get; set; } = string.Empty;
    }
}