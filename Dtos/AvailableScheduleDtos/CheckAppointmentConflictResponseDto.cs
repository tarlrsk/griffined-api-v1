using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class CheckAppointmentConflictResponseDto
    {
        public bool IsConflict { get; set; }
        public List<ConflictScheduleResponseDto> ConflictSchedules { get; set; } = new List<ConflictScheduleResponseDto>();
    }
}