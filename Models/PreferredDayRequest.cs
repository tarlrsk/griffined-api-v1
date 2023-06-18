using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class PreferredDayRequest
    {
        public int id { get; set; }
        public string day { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        public string fromTime { get; set; } = string.Empty;
        private TimeOnly _toTime;
        public string toTime { get; set; } = string.Empty;
        public ICollection<RegistrationRequest> registrationRequests = new List<RegistrationRequest>();
    }
}