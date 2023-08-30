using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RequestedCourseResponseDto
    {
        public string? Section { get; set; }
        public int? StudyCourseId { get; set;}
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int? LevelId { get; set; }
        public string? Level { get; set; }
        public int TotalHours { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public StudyCourseType StudyCourseType { get; set; }
        public Method Method { get; set; }
        public List<RequestedSubjectResponseDto> subjects { get; set; } = new List<RequestedSubjectResponseDto>();
    }
}