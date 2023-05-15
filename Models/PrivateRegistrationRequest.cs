using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [Index(nameof(status), nameof(EAStatus), nameof(paymentStatus), Name = "Index_statuses")]
    public class PrivateRegistrationRequest
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string section { get; set; } = string.Empty;
        [Required]
        public string courseType { get; set; } = string.Empty;
        [Required]
        public DateTime dateCreated { get; set; } = DateTime.Now;
        [Required]
        public RegistrationRequestStatus status { get; set; } = RegistrationRequestStatus.None;
        [Required]
        public EARequestStatus EAStatus { get; set; } = EARequestStatus.InProgress;
        [Required]
        public PaymentStatus paymentStatus { get; set; } = PaymentStatus.None;
        public int? takenByEPId { get; set; }
        public int? takenByEAId { get; set; }
        public int? takenByOAId { get; set; }
        public string? EPRemark1 { get; set; }
        public string? EPRemark2 { get; set; }
        public string? EARemark { get; set; }
        public string? OARemark { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public ICollection<Student> students { get; set; } = new List<Student>();
        public ICollection<PrivateCourse> courses = new List<PrivateCourse>();
        public List<PrivateRegistrationRequestInfo> privateRegistrationRequestInfos { get; set; } = new List<PrivateRegistrationRequestInfo>();
        public ICollection<Payment> payments = new List<Payment>();
    }
}