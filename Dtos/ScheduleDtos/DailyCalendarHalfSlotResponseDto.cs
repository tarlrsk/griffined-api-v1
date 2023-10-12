using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class DailyCalendarHalfSlotResponseDto
    {
        public DailyCalendarSlotResponseDto? FirstHalf { get; set; }
        public DailyCalendarSlotResponseDto? SecondHalf { get; set; }
    }
}