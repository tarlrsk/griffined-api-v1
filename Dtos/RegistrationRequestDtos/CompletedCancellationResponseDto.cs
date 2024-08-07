using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class CompletedCancellationResponseDto
    {
        public int RequestId { get; set; }
        public string Section { get; set; } = string.Empty;
        public RegistrationRequestType RegistrationRequestType { get; set; }
        public StudyCourseType? StudyCourseType { get; set; }
        public List<StudentNameResponseDto> Members { get; set; } = new List<StudentNameResponseDto>();
        public List<PreferredDayResponseDto> PreferredDays { get; set; } = new List<PreferredDayResponseDto>();
        public List<RequestedCourseResponseDto> Courses { get; set; } = new List<RequestedCourseResponseDto>();
        public List<ScheduleResponseDto> Schedules { get; set; } = new List<ScheduleResponseDto>();
        public PaymentType? PaymentType { get; set; }
        public List<FilesResponseDto> PaymentFiles { get; set; } = new List<FilesResponseDto>();
        public RegistrationStatus RegistrationStatus { get; set; }
        public List<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool PaymentError { get; set; }
        public bool ScheduleError { get; set; }
        public bool NewCourseDetailError { get; set; }
        public bool HasSchedule { get; set; }
        public StaffNameOnlyResponseDto? ByEC { get; set; }
        public StaffNameOnlyResponseDto? ByEA { get; set; }
        public StaffNameOnlyResponseDto? ByOA { get; set; }
        public StaffNameOnlyResponseDto? CancelledBy { get; set; }
    }
}