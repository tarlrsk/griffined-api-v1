namespace griffined_api.Dtos.StudyCourseDtos
{
    public class ClassProgressResponseDto
    {
        public int StudyCourseId { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
    }
}