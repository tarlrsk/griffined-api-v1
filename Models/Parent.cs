using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class Parent
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }

        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? FullName { get { return FirstName + " " + LastName; } }
        public string? Relationship { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Line { get; set; } = string.Empty;

        [ForeignKey(nameof(StudentId))]
        public virtual Student? Student { get; set; }


    }
}