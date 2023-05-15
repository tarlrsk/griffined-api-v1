using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PaymentDtos
{
    public class AddPrivatePaymentDto
    {
        public int privateRegReqId { get; set; }
        public List<AddPaymentFileDto> payment { get; set; } = new List<AddPaymentFileDto>();
        public string paymentType { get; set; } = string.Empty;
    }
}