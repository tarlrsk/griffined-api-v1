namespace griffined_api.Dtos.StudentReportDtos
{
    public class StudentReportWithStudentResponseDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public List<StudySubjectReportResponseDto> Subjects { get; set; } = new List<StudySubjectReportResponseDto>();
    }
}