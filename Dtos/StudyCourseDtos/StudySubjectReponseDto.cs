using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudySubjectResponseDto
    {
        public int StudySubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
    }
}