using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class ProfilePicture
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }

        public string FileName { get; set; } = String.Empty;
        public string ObjectName { get; set; } = String.Empty; // specific path to the file

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = new Student();
    }
}