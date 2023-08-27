using griffined_api.Dtos.StudentReportDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudyCourseByStudentIdResponseDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;
        public string StudentNickname { get; set; } = string.Empty;
        public int StudyCourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public StudyCourseStatus Status { get; set; }
        public List<StudySubjectReportResponseDto> Reports { get; set; } = new();
    }
}