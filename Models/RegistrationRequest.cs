using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RegistrationRequest
    {
        public int id { get; set; }
        public string section { get; set; } = string.Empty;
        private DateTime _date;
        public string date { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public _PaymentTypeEnum paymentType { get; set; }
        public _PaymentStatusEnum paymentStatus { get; set; }
        public int byECId { get; set; }
        public int byEAId { get; set; }
        public int byOAId { get; set; }
        public int studentAddingRequestId { get; set; }
        public StudentAddingRequest? studentAddingRequest { get; set; }
    }
}