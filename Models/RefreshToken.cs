using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RefreshToken
    {
        public string token { get; set; } = string.Empty;
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime expires { get; set; } = DateTime.Now.AddDays(1);
    }
}