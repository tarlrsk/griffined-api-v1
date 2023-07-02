using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAdditionalFile
    {
        public int id { get; set; }
        public int? studentId { get; set; }

        public string fileName { get; set; } = string.Empty;

        [ForeignKey(nameof(studentId))]
        public virtual Student student { get; set; } = new Student();
    }
}