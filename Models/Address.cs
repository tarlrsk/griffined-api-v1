using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace griffined_api.Models
{
    public class Address
    {
        public int id { get; set; }
        public int? studentId { get; set; }

        public string address { get; set; } = string.Empty;
        public string subdistrict { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string province { get; set; } = string.Empty;
        public string zipcode { get; set; } = string.Empty;

        [ForeignKey(nameof(studentId))]
        public virtual Student? student { get; set; }
    }
}