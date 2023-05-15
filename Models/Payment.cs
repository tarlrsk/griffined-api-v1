using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Payment
    {
        [Required]
        public int id { get; set; }
        public List<PaymentFile> payment { get; set; } = new List<PaymentFile>();
        [Required]
        public string paymentType { get; set; } = string.Empty;
        public int privateRegReqId { get; set; }
        public PrivateRegistrationRequest? privateRegReq { get; set; }
        public int groupRegReqId { get; set; }
        //public GroupRegistrationRequest? groupRegReq { get; set; }
    }
}