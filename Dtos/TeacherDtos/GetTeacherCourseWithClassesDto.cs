using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.TeacherDtos
{
    public class GetTeacherCourseWithClassesDto
    {
        public GetPrivateRegistrationRequestDto request { get; set; } = new GetPrivateRegistrationRequestDto();
        public GetPrivateCourseDto course { get; set; } = new GetPrivateCourseDto();
        public List<GetPrivateClassWithNameDto> classes { get; set; } = new List<GetPrivateClassWithNameDto>();
    }
}