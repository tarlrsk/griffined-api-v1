using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewCoursesRequestDto
    {
        public string? sectionName { get; set; }
        [Required]
        public required List<int> memberIds { get; set; }
        [Required]
        public string type {get; set;} = string.Empty;
        public List<PreferredDayRequestDto> preferredDays { get; set; } = new List<PreferredDayRequestDto>();
        public List<NewRequestedCourseDto>? courses { get; set; }
    }
}