using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PaymentDtos
{
    public class GetPrivatePaymentDto
    {
        public int id { get; set; }
        public int privateRegReqId { get; set; }
        public List<GetPaymentFileDto> payment { get; set; } = new List<GetPaymentFileDto>();
        public string paymentType { get; set; } = string.Empty;
    }
}