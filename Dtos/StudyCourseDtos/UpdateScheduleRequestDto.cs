namespace griffined_api.Services.StudyCourseService
{
    public class UpdateScheduleRequestDto
    {
        [Required]
        public string Date { get; set; } = string.Empty;
        [Required]
        public string FromTime { get; set; } = string.Empty;
        [Required]
        public string ToTime { get; set; } = string.Empty;
        [Required]
        public int StudySubjectId { get; set; }
        [Required]
        public int TeacherId { get; set; }
    }
}