namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RequestedSubjectResponseDto
    {
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int? StudySubjectId { get; set; }
        public double? Hour { get; set; }
    }
}