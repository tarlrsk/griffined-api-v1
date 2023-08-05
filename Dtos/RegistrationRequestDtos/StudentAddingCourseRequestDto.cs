using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class StudentAddingCourseRequestDto
    {
        [Required]
        public int StudyCourseId { get; set; }
        [Required]
        public List<int> StudySubjectIds { get; set; } = new List<int>();
    }
}