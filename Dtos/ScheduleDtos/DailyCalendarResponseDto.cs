using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class DailyCalendarResponseDto
    {
        public int? Id { get; set; }
        public int? TeacherId { get; set; }
        public string? Teacher { get; set; }
        public List<DailyCalendarHalfSlotResponseDto?> HourSlots { get; set; } = new List<DailyCalendarHalfSlotResponseDto?>();
    }
}