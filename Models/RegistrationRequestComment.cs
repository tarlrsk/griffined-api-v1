using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class RegistrationRequestComment
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? StaffId { get; set; }

        public string Comment { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest RegistrationRequest { get; set; }

        [ForeignKey(nameof(StaffId))]
        public virtual Staff Staff { get; set; }
    }
}