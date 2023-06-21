using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Parent
    {
        public int id { get; set; }
        public int? studentId { get; set; }

        public string fName { get; set; } = string.Empty;
        public string lName { get; set; } = string.Empty;
        public string fullName { get { return fName + " " + lName; } }
        public string relationship { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string line { get; set; } = string.Empty;

        [ForeignKey(nameof(studentId))]
        public virtual Student student { get; set; } = new Student();


    }
}