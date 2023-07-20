using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewCoursesRequestDto
    {
        public string? SectionName { get; set; }
        [Required]
        public List<int> MemberIds { get; set; } = new List<int>();
        [Required]
        public string Type { get; set; } = string.Empty;
        public List<PreferredDayRequestDto> PreferredDays { get; set; } = new List<PreferredDayRequestDto>();
        public List<NewRequestedCourseDto>? Courses { get; set; }
        public List<String> Comments { get; set; } = new List<String>();
    }
}