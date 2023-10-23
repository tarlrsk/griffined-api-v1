using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictScheduleDetailReponseDto
    {
        public int? ScheduleId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ConflictMemberResponseDto> ConflictMembers { get; set; } = new List<ConflictMemberResponseDto>();
    }
}