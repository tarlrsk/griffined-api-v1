using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RegistrationRequestPendingOAResponseDto
    {
        public int RequestId { get; set; }
        public string Section { get; set; } = string.Empty;
        public RegistrationRequestType RegistrationRequestType { get; set; }
        public StudyCourseType? StudyCourseType { get; set; }
        public List<StudentNameResponseDto> Members { get; set; } = new List<StudentNameResponseDto>();
        public List<RequestedCourseResponseDto> Courses { get; set; } = new List<RequestedCourseResponseDto>();
        public List<ScheduleResponseDto> Schedules { get; set; } = new List<ScheduleResponseDto>();
        public PaymentType? PaymentType { get; set; }
        public List<FilesResponseDto> PaymentFiles { get; set; } = new List<FilesResponseDto>();
        public RegistrationStatus RegistrationStatus { get; set; }
        public List<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
    }
}