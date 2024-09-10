using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class Manday
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TeacherId { get; set; }

        public int Year { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public Teacher? Teacher { get; set; }

        public virtual ICollection<WorkTime> WorkTimes { get; set; }
    }
}