using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateRegReqWithInfoDtos
{
    public class GetPrivateRegReqWithInfoDto
    {
        public List<GetStudentNameDto> students { get; set; } = new List<GetStudentNameDto>();
        public GetPrivateRegistrationRequestDto request { get; set; } = new GetPrivateRegistrationRequestDto();
        public List<GetPrivateRegReqInformationDto> information { get; set; } = new List<GetPrivateRegReqInformationDto>();
    }
}