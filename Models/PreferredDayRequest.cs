using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class PreferredDayRequest
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }

        public string Day { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        public string FromTime { get; set; } = string.Empty;
        private TimeOnly _toTime;
        public string ToTime { get; set; } = string.Empty;

        [ForeignKey(nameof(RegistrationRequestId))]
        public RegistrationRequest RegistrationRequest { get; set; } = new RegistrationRequest();
    }
}