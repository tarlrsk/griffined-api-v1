using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateRegistrationRequestDtos
{
    public class GetPrivateRegistrationRequestDto
    {
        public int id { get; set; }
        public string section { get; set; } = string.Empty;
        public string courseType { get; set; } = string.Empty;
        public RegistrationRequestStatus status { get; set; } = RegistrationRequestStatus.None;
        public EARequestStatus EAStatus { get; set; } = EARequestStatus.InProgress;
        public PaymentStatus paymentStatus { get; set; } = PaymentStatus.None;
        public DateTime dateCreated { get; set; } = DateTime.Now;
        public string? EPRemark1 { get; set; }
        public string? EPRemark2 { get; set; }
        public string? EARemark { get; set; }
        public string? OARemark { get; set; }
        public int takenByEPId { get; set; }
        public int takenByEAId { get; set; }
        public int takenByOAId { get; set; }
    }
}