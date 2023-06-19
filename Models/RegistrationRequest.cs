using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RegistrationRequest
    {
        public int id { get; set; }
        public string section { get; set; } = string.Empty;
        private DateTime _date;
        public string date { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public ICollection<PaymentFile> paymentFiles { get; set; } = new List<PaymentFile>();
        public PaymentType paymentType { get; set; }
        public PaymentStatus paymentStatus { get; set; }
        public int byECId { get; set; }
        public int byEAId { get; set; }
        public int byOAId { get; set; }
        public StudentAddingRequest? studentAddingRequest { get; set; }
        public ICollection<NewCourseRequest> newCourseRequests { get; set; } = new List<NewCourseRequest>();
        public ICollection<RegistrationRequestMember> registrationRequestMembers { get; set; } = new List<RegistrationRequestMember>();
        public ICollection<PreferredDayRequest> preferredDayRequests { get; set; } = new List<PreferredDayRequest>();

    }
}