using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewCoursesRequestDto
    {
        public string? Section { get; set; }
        [Required]
        public List<int> MemberIds { get; set; } = new List<int>();
        [Required]
        public StudyCourseType Type { get; set; }
        public List<PreferredDayRequestDto> PreferredDays { get; set; } = new List<PreferredDayRequestDto>();
        public List<NewRequestedCourseDto>? Courses { get; set; }
        public List<String> Comments { get; set; } = new List<String>();
    }
}