using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class EaStudentManagementRequestDto
    {
        [Required]
        public string StudentCode { get; set; } = string.Empty;

        [Required]
        public int StudyCourseId { get; set; }

        [Required]
        public List<int> StudySubjectIds { get; set; } = new();
    }
}