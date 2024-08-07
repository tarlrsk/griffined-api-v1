using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class StudentAdditionalFile
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty; // specific path to the file

        [ForeignKey(nameof(StudentId))]
        public virtual Student? Student { get; set; }
    }
}