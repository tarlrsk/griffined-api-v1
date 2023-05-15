using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api
{
    [Index(nameof(attendance), Name = "Index_attendance")]
    public class StudentPrivateClass
    {
        [Required]
        public int id { get; set; }
        [Required]
        public int studentId { get; set; }
        [Required]
        public Student student { get; set; } = new Student();
        [Required]
        public int classId { get; set; }
        [Required]
        public PrivateClass privateClass { get; set; } = new PrivateClass();
        [Required]
        public Attendance attendance { get; set; } = Attendance.None;
    }
}