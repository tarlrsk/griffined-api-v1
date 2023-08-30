using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RegistrationRequestPendingEADetail2ResponseDto
    {
        public int RequestId { get; set; }
        public string Section { get; set; } = string.Empty;
        public RegistrationRequestType RegistrationRequestType { get; set; }
        public StudyCourseType StudyCourseType { get; set; }
        public List<StudentNameResponseDto> Members { get; set; } = new List<StudentNameResponseDto>();
        public StaffNameOnlyResponseDto TakenByEA { get; set; } = new();
        public List<PreferredDayResponseDto> PreferredDays { get; set; } = new List<PreferredDayResponseDto>();
        public List<RequestedCourseResponseDto> Courses { get; set; } = new List<RequestedCourseResponseDto>();
        public List<ScheduleResponseDto> Schedules { get; set; } = new List<ScheduleResponseDto>();
        public RegistrationStatus RegistrationStatus { get; set; }
        public List<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
    }
}