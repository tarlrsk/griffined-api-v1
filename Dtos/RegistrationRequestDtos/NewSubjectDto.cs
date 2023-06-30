using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewSubjectDto
    {
        public string subject { get; set; } = string.Empty;
        public int hour { get; set; }
    }
}