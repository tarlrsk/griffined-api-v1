namespace griffined_api.Dtos.StudentReportDtos
{
    public class StudentReportDetailRequestDto
    {
        [Required]
        public string StudentCode { get; set; } = string.Empty;

        [Required]
        public int StudySubjectId { get; set; }

        [Required]
        public Progression Progression { get; set; }

    }
}