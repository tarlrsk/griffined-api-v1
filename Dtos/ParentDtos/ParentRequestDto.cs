using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ParentDtos
{
    public class ParentRequestDto
    {
        public string fName { get; set; } = string.Empty;
        public string lName { get; set; } = string.Empty;
        public string fullName { get { return fName + " " + lName; } }
        public string relationship { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string line { get; set; } = string.Empty;
    }

}