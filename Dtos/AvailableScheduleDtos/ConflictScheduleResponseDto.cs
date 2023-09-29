using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictScheduleResponseDto
    {
        public string ConflictMessage { get; set; } = string.Empty;
        public List<ConflictMemberResponseDto> Members { get; set; } = new List<ConflictMemberResponseDto>();
    }
}