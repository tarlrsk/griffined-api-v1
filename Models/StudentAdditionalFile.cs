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
        public int Id { get; set; }
        public int? StudentId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = new Student();
    }
}