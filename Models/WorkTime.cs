using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class WorkTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? MandayId { get; set; }

        public int Quarter { get; set; }

        public DayOfWeek Day { get; set; }

        [ForeignKey(nameof(MandayId))]
        public Manday? Manday { get; set; }
    }
}