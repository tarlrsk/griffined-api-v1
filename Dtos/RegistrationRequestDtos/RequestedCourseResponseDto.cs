using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RequestedCourseResponseDto
    {
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int? LevelId { get; set; }
        public string? Level { get; set; }
        public int TotalHours { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public Method Method { get; set; }
        public List<RequestedSubjectResponseDto> subjects { get; set; } = new List<RequestedSubjectResponseDto>();
    }
}