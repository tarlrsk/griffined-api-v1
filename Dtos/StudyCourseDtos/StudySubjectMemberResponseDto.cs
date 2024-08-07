namespace griffined_api.Services.StudyCourseService
{
    public class StudySubjectMemberResponseDto
    {
        public List<StudentStudySubjectMemberResponseDto> Students { get; set; } = new();
        public List<TeacherStudySubjectMemberResponseDto> Teachers { get; set; } = new();
    }
}