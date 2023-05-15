using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentClassCancellationRequest
    {
        [Required]
        public int id { get; set; }
        [Required]
        public int studentId { get; set; }
        [Required]
        public Student student { get; set; } = new Student();
        [Required]
        public int privateClassId { get; set; }
        [Required]
        public PrivateClass privateClass { get; set; } = new PrivateClass();
        public string? studentRemark { get; set; }
        public string? OARemark { get; set; }
    }
}