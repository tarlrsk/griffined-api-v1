using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RegistrationRequestResponseDto
    {
        public int requestId { get; set; }
        public List<StudentNameResponseDto> members {get; set;} = new List<StudentNameResponseDto>();

}
}