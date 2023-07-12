using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StaffDtos
{
    public class StaffNameOnlyResponseDto
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Nickname {get; set;} = string.Empty;

    }
}