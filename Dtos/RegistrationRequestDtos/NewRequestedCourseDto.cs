namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewRequestedCourseDto
    {
        [Required]
        public string Course { get; set; } = string.Empty;
        public string? Level { get; set; }
        [Required]
        public Method Method { get; set; }
        [Required]
        public double TotalHours { get; set; }
        [Required]
        public string StartDate { get; set; } = string.Empty;
        [Required]
        public string EndDate { get; set; } = string.Empty;
        public List<NewSubjectDto>? Subjects { get; set; }
    }
}