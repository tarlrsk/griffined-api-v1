using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using Newtonsoft.Json;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class RequestedScheduleRequestDto
    {
        [Required]
        public List<int> StudentIds { get; set; } = new List<int>();
        [Required]
        public List<int> CurrentStudySubjectId { get; set; } = new List<int>();
        [Required]
        public List<string> Dates { get; set; } = new List<string>();
        [Required]
        public string FromTime { get; set; } = string.Empty;
        [Required]
        public string ToTime { get; set; } = string.Empty;
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public int RequestedStudyCourseId { get; set; }
        public int? RequestedStudySubjectId { get; set; }
        public List<LocalScheduleRequestDto> LocalSchedule { get; set; } = new List<LocalScheduleRequestDto>();
    }
}