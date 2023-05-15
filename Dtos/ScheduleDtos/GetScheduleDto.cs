using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class GetScheduleDto
    {
        public int id { get; set; }
        public int requestId { get; set; }
        public RegistrationRequestStatus requestStatus { get; set; } = RegistrationRequestStatus.None;
        public GetPrivateCourseDto course { get; set; } = new GetPrivateCourseDto();
        public List<GetPrivateClassWithNameDto> classes { get; set; } = new List<GetPrivateClassWithNameDto>();
    }
}