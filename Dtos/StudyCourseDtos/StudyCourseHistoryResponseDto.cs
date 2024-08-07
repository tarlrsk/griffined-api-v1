namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudyCourseHistoryResponseDto
    {
        public string Date { get; set; } = string.Empty;
        public StudyCourseHistoryType RecordType { get; set; }
        public string Record { get; set; } = string.Empty;
    }
}