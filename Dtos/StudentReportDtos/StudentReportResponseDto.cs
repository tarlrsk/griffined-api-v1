using griffined_api.Dtos.ScheduleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentReportDtos
{
    public class StudentReportResponseDto
    {
        public int StudyCourseId { get; set; }
        public string course { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public List<StudySubjectReportResponseDto> Report { get; set; } = new List<StudySubjectReportResponseDto>();

    }
}