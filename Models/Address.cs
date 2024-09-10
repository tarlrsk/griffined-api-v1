using System.ComponentModel.DataAnnotations.Schema;


namespace griffined_api.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }

        public string? address { get; set; } = string.Empty;
        public string? Subdistrict { get; set; } = string.Empty;
        public string? District { get; set; } = string.Empty;
        public string? Province { get; set; } = string.Empty;
        public string? Zipcode { get; set; } = string.Empty;

        [ForeignKey(nameof(StudentId))]
        public virtual Student? Student { get; set; }
    }
}