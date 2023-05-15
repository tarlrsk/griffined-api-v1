using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PaymentDtos
{
    public class UpdatePrivatePaymentDto
    {
        public int privateRegReqId { get; set; }
        public List<UpdatePaymentFileDto> payment { get; set; } = new List<UpdatePaymentFileDto>();
        public string paymentType { get; set; } = string.Empty;
    }
}
