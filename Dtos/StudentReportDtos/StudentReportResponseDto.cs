namespace griffined_api.Dtos.StudentReportDtos
{
    public class StudentReportResponseDto
    {
        public int StudyCourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public List<StudentReportWithStudentResponseDto> Students { get; set; } = new List<StudentReportWithStudentResponseDto>();
    }
}