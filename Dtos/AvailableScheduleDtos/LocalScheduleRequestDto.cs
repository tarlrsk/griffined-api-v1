using Newtonsoft.Json;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class LocalScheduleRequestDto
    {
        [Required]
        public string Date { get; set; } = string.Empty;
        [Required]
        public string FromTime { get; set; } = string.Empty;
        [Required]
        public string ToTime { get; set; } = string.Empty;
        public int? StudySubjectId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int SubjectId { get; set; }
        public int? LevelId { get; set; }
        [Required]
        public int TeacherId { get; set; }
    }
}