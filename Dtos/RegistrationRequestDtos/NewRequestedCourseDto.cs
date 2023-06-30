using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewRequestedCourseDto
    {
        public string course { get; set; } = string.Empty;
        public string? level { get; set; }
        public int totalHours { get; set; }
        public string startDate { get; set; } = string.Empty;
        public string endDate { get; set; } = string.Empty;
        public List<NewSubjectDto>? subjects {get; set;}
    }
}