using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class StudentAddingConflictResponseDto
    {
        public bool IsConflict { get; set; }
        public List<ConflictScheduleResponseDto> ConflictMessages { get; set; } = new List<ConflictScheduleResponseDto>();
    }
}