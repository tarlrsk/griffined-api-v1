using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StaffDtos
{
    public class StaffNameOnlyResponseDto
    {
        public int staffId { get; set; }
        public string fullName { get; set; } = string.Empty;
        public string nickname {get; set;} = string.Empty;

    }
}