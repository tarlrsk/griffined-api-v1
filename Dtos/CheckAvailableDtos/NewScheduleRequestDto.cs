using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using griffined_api.Dtos.CheckTimeConflictDtos;
using Newtonsoft.Json;

namespace griffined_api.Dtos.CheckAvailableDtos
{
    public class AddedScheduleRequestDto
    {
        [Required]
        public List<int> CurrentStudySubjectId { get; set; } = new List<int>();
        [Required]
        public List<string> Dates { get; set; } = new List<string>();
        [Required]
        public string Time { get; set; } = string.Empty;
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public int RequestedStudyCourseId { get; set; }
        public int? RequestedStudySubjectId { get; set; }
        public List<LocalScheduleResponseDto> LocalSchedule {get; set;} = new List<LocalScheduleResponseDto>();
    }
}