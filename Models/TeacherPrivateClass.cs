using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [Index(nameof(status), Name = "Index_status")]
    public class TeacherPrivateClass
    {
        [Required]
        public int id { get; set; }
        [Required]
        public int teacherId { get; set; }
        [Required]
        public Teacher teacher { get; set; } = new Teacher();
        [Required]
        public string workType { get; set; } = string.Empty;
        [Required]
        public int privateClassId { get; set; }
        [Required]
        public PrivateClass privateClass { get; set; } = new PrivateClass();
        [Required]
        public TeacherClassStatus status { get; set; } = TeacherClassStatus.Incomplete;
    }
}