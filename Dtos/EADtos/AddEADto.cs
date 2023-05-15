using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.EADtos
{
    public class AddEADto
    {
        [Required]
        public string fName { get; set; } = string.Empty;
        [Required]
        public string lName { get; set; } = string.Empty;
        [Required]
        public string nickname { get; set; } = string.Empty;
        [Required]
        public string phone { get; set; } = string.Empty;
        public string role { get; set; } = "EA";
        [Required]
        public string line { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
    }
}