using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class StudyAddingRequestDto
    {
        [Required]
        public required List<int> MemberIds { get; set; }
        [Required]
        public PaymentType PaymentType { get; set; }
        [Required]
        public List<StudyAddingCourseRequestDto> StudyCourse { get; set; } = new List<StudyAddingCourseRequestDto>();
        [Required]
        public List<String> Comments { get; set; } = new List<String>();
    }
}