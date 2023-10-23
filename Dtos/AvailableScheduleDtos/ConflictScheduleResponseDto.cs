using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictScheduleResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public List<ConflictMemberResponseDto> ConflictMembers { get; set; } = new List<ConflictMemberResponseDto>();
        public int? StudyCourseId { get; set; }
        public int? AppointmentId { get; set; }
    }
}