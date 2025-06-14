namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudySubjectWithMembersResponseDto
    {
        public int StudySubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public List<StudySubjectMemberWithReportsResponseDto> Members { get; set; } = new();
    }
}