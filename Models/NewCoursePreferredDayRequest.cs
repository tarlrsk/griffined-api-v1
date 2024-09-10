using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class NewCoursePreferredDayRequest
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }

        public string Day { get; set; } = string.Empty;

        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        [ForeignKey(nameof(RegistrationRequestId))]
        public RegistrationRequest RegistrationRequest { get; set; }
    }
}