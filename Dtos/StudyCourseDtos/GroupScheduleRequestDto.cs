namespace griffined_api.Dtos.StudyCourseDtos
{
    public class GroupScheduleRequestDto
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public List<int> SubjectIds { get; set; } = new List<int>();
        public int? LevelId { get; set; }
        [Required]
        public string Section { get; set; } = string.Empty;
        [Required]
        public string StartDate { get; set; } = string.Empty;
        [Required]
        public string EndDate { get; set; } = string.Empty;
        public int HourPerDay { get; set; }
        [Required]
        public double TotalHours { get; set; }
        public Method Method { get; set; }
        public List<NewScheduleRequestDto> Schedules { get; set; } = new List<NewScheduleRequestDto>();

    }
}