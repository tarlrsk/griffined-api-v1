using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class ClassCancellationRequestResponseDto
    {
        public int RequestId { get; set; }
        public int StudyCourseId { get; set; }
        public string Section { get; set; } = string.Empty;
        public StudyCourseType StudyCourseType { get; set; }
        public virtual CancellationRole RequestedRole { get; set; }
        public RequestedByResponseDto RequestedBy { get; set; } = new RequestedByResponseDto();
        public string Course { get; set; } = string.Empty;
        public string? Level { get; set; }
        public List<StudySubjectResponseDto> StudySubjects { get; set; } = new List<StudySubjectResponseDto>();
        public string RequestedDate { get; set; } = string.Empty;
        public string CancelledDate { get; set; } = string.Empty;
        public string CancelledFromTime { get; set; } = string.Empty;
        public string CancelledToTime { get; set; } = string.Empty;
        public StaffNameOnlyResponseDto? TakenByEA { get; set; }
        public ClassCancellationRequestStatus Status { get; set; }
    }
}