namespace griffined_api.Dtos.StudyCourseDtos
{
    public class NewStudyClassScheduleRequestDto
    {
        [Required]
        public int ClassNo { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public string Date { get; set; } = string.Empty;
        [Required]
        public string FromTime { get; set; } = string.Empty;
        [Required]
        public string ToTime { get; set; } = string.Empty;
    }
}