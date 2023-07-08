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
        public string Section { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public RegistrationStatus RegistrationStatus { get; set; }
        public PaymentType? PaymentType { get; set; }
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