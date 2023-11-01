using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class DailyCalendarSlotResponseDto
    {
        public int ScheduleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Room { get; set; }
        public DailyCalendarType Type { get; set; }
        public string Time { get; set; } = string.Empty;
    }
}