using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class GroupScheduleRequestDto
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public List<int> SubjectIds { get; set; } = new List<int>();
        public int LevelId { get; set; }
        [Required]
        public string Section{ get; set; } = string.Empty;
        [Required]
        public string StartDate { get; set; } = string.Empty;
        [Required]
        public string EndDate { get; set; } = string.Empty;
        public string HourPerDay { get; set; } = string.Empty;
        [Required]
        public int TotalHours { get; set; }
        public Method Method { get; set; }
        public List<ScheduleRequestDto> schedules = new List<ScheduleRequestDto>();

    }
}