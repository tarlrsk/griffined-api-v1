namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class CancellationInfoResponseDto
    {
        public int ClassId { get; set; }
        public int ClassNo { get; set; }
        public int StudyCourseId { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int StudySubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;
    }
}