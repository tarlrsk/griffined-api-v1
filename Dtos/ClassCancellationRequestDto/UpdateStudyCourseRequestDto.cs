namespace griffined_api.Services.StudyCourseService
{
    public class UpdateStudyCourseRequestDto
    {
        [Required]
        public List<int> StudySubjectIds { get; set; } = new List<int>();
        [Required]
        public List<int> RemoveStudyClassId { get; set; } = new List<int>();
        [Required]
        public List<UpdateScheduleRequestDto> NewSchedule { get; set; } = new List<UpdateScheduleRequestDto>();
    }
}