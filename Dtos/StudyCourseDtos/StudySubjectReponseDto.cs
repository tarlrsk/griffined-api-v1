namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudySubjectResponseDto
    {
        public int StudySubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public double? Hour { get; set; }
    }
}