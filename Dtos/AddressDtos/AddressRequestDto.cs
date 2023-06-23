using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AddressDtos
{
    public class AddressRequestDto
    {
        public string address { get; set; } = string.Empty;
        public string subdistrict { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string province { get; set; } = string.Empty;
        public string zipcode { get; set; } = string.Empty;
    }
}