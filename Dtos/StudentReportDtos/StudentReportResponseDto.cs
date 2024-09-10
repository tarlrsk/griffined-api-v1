namespace griffined_api.Dtos.StudentReportDtos
{
    public class StudentReportResponseDto
    {
        public int StudyCourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public double Progress { get; set; }
        public List<StudentReportWithStudentResponseDto> Students { get; set; } = new List<StudentReportWithStudentResponseDto>();
    }
}