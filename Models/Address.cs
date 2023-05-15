using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Address
    {
        [Required]
        public int id { get; set; }
        public string address { get; set; } = string.Empty;
        public string subdistrict { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string province { get; set; } = string.Empty;
        public string zipcode { get; set; } = string.Empty;
        public Student? student { get; set; }
        public int studentId { get; set; }
    }
}