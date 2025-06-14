using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class StaffNotification
    {
        public int Id { get; set; }
        public int? StaffId { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? CancellationRequestId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool HasRead { get; set; }

        public virtual StaffNotificationType Type { get; set; }

        [ForeignKey(nameof(StaffId))]
        public virtual Staff Staff { get; set; } = new Staff();

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest? RegistrationRequest { get; set; }

        [ForeignKey(nameof(CancellationRequestId))]
        public virtual ClassCancellationRequest? CancellationRequest { get; set; }
    }
}