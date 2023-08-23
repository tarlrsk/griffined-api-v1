using griffined_api.Dtos.StudyCourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentReportDtos
{
    public class StudySubjectReportResponseDto
    {
        public StudySubjectResponseDto StudySubject { get; set; } = new StudySubjectResponseDto();
        public ReportFileResponseDto? FiftyPercentReport { get; set; }
        public ReportFileResponseDto? HundredPercentReport { get; set; }
        public ReportFileResponseDto? SpecialReport { get; set; }
    }
}