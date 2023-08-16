using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class EnrolledStudyCourseResponseDto
    {
        public int StudyCourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public List<StudySubjectResponseDto> StudySubjects { get; set; } = new List<StudySubjectResponseDto>();
        public StudyCourseType StudyCourseType { get; set; }
        public int? LevelId { get; set; }
        public string? Level { get; set; }
        public string Section { get; set; } = string.Empty;
    }
}