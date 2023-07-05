using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RegistrationRequestResponseDto
    {
        public int requestId { get; set; }
        public List<StudentNameResponseDto> members { get; set; } = new List<StudentNameResponseDto>();
        public string section { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public RegistrationStatus registrationStatus { get; set; }
        public PaymentType? paymentType { get; set; }
        public PaymentStatus? paymentStatus { get; set; }
        public DateTime createdDate { get; set; }
        public bool paymentError { get; set; }
        public bool scheduleError { get; set; }
        public bool newCourseDetailError { get; set; }
        public bool hasSchedule { get; set; }
        public StaffNameOnlyResponseDto? byEC { get; set; }
        public StaffNameOnlyResponseDto? byEA { get; set; }
        public StaffNameOnlyResponseDto? byOA { get; set; }
        public StaffNameOnlyResponseDto? cancelledBy { get; set; }
    }
}