using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PaymentDtos
{
    public class UpdatePaymentFileDto
    {
        public int id { get; set; }
        public string file { get; set; } = string.Empty;
    }
}