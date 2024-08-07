using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class ClassCancellationRequestDetailResponseDto
    {
        public int RequestId { get; set; }
        public int StudyCourseId { get; set; }
        public string Section { get; set; } = string.Empty;
        public StudyCourseType StudyCourseType { get; set; }
        public virtual CancellationRole RequestedRole { get; set; }
        public RequestedByWithContactResponseDto RequestedBy { get; set; } = new RequestedByWithContactResponseDto();
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public string? Level { get; set; }
        public List<StudySubjectResponseDto> StudySubjects { get; set; } = new List<StudySubjectResponseDto>();
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public double TotalHour { get; set; }
        public Method Method { get; set; }
        public string RequestedDate { get; set; } = string.Empty;
        public StaffNameOnlyResponseDto? TakenByEA { get; set; }
        public ClassCancellationRequestStatus Status { get; set; }
        public string? RejectedReason { get; set; }
        public CancellationInfoResponseDto RequestedClass { get; set; } = new CancellationInfoResponseDto();
        public List<ScheduleResponseDto> Schedules { get; set; } = new List<ScheduleResponseDto>();
    }
}