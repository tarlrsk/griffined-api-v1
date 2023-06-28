using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewCoursesRequestDto
    {
        public string sectionName { get; set; } = string.Empty; //TODO Nullable
        [Required]
        public required List<int> memberIds { get; set; }
        [Required]
        public string type {get; set;} = string.Empty;
        [Required]
        public List<PreferredDayRequestDto> preferredDays { get; set; } = new List<PreferredDayRequestDto>();
        [Required]
        public List<NewRequestedCourseDto>? courses { get; set; }
    }
}