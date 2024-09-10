using griffined_api.Dtos.StudyCourseDtos;

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