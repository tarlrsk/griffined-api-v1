using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewCoursesRequestDto
    {
        [Required]
        public string sectionName { get; set; } = string.Empty;
        [Required]
        public required List<int> memberIds { get; set; }
        public List<PreferredDayRequestDto>? preferredDays { get; set; }
        public List<NewCourseDto>? courses { get; set; }
    }
}