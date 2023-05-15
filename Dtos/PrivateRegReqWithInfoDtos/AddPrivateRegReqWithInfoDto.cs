using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateRegReqWithInfoDtos
{
    public class AddPrivateRegReqWithInfoDto
    {
        public List<int> studentIds { get; set; } = new List<int>();
        public AddPrivateRegistrationRequestDto request { get; set; } = new AddPrivateRegistrationRequestDto();
        public List<AddPrivateRegReqInformationDto> information { get; set; } = new List<AddPrivateRegReqInformationDto>();
    }
}