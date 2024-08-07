namespace griffined_api.Dtos.StudentReportDtos
{
    public class AddStudentReportRequestDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public required IFormFile ReportData { get; set; }
    }
}