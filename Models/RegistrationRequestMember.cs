using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class RegistrationRequestMember
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? RegistrationRequestId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest RegistrationRequest { get; set; }
    }
}