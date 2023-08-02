using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AddressDtos
{
    public class AddressRequestDto
    {
        public string? Address { get; set; } = string.Empty;
        public string? Subdistrict { get; set; } = string.Empty;
        public string? District { get; set; } = string.Empty;
        public string? Province { get; set; } = string.Empty;
        public string? Zipcode { get; set; } = string.Empty;
    }
}