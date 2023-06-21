using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RegistrationRequest
    {
        public int id { get; set; }
        public int byECId { get; set; }
        public int byEAId { get; set; }
        public int byOAId { get; set; }

        public string section { get; set; } = string.Empty;
        private DateTime _date;
        public string date { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public virtual PaymentType paymentType { get; set; }
        public virtual PaymentStatus paymentStatus { get; set; }

        public virtual StudentAddingRequest? studentAddingRequest { get; set; }
        public virtual ICollection<Payment> payment { get; set; } = new List<Payment>();
        public virtual ICollection<NewCourseRequest> newCourseRequests { get; set; } = new List<NewCourseRequest>();
        public virtual ICollection<RegistrationRequestMember> registrationRequestMembers { get; set; } = new List<RegistrationRequestMember>();
        public virtual ICollection<PreferredDayRequest> preferredDayRequests { get; set; } = new List<PreferredDayRequest>();

    }
}