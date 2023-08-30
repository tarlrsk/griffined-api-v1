using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RegistrationRequestResponseDto
    {
        public int RequestId { get; set; }
        public List<StudentNameResponseDto> Members { get; set; } = new List<StudentNameResponseDto>();
        public string Section { get; set; } = "Multiple";
        public List<StudyCourseType> StudyCourseType { get; set; } = new List<StudyCourseType>();
        public RegistrationRequestType Type { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; }
        public PaymentType? PaymentType { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool PaymentError { get; set; }
        public bool ScheduleError { get; set; }
        public bool NewCourseDetailError { get; set; }
        public bool HasSchedule { get; set; }
        public StaffNameOnlyResponseDto? ByEC { get; set; }
        public StaffNameOnlyResponseDto? TakenByEA { get; set; }
        public StaffNameOnlyResponseDto? ScheduledBy { get; set; }
        public StaffNameOnlyResponseDto? ByOA { get; set; }
        public StaffNameOnlyResponseDto? CancelledBy { get; set; }
    }
}