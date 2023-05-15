using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class GetStudentCourseWithClassesDto
    {
        public GetPrivateRegistrationRequestDto request { get; set; } = new GetPrivateRegistrationRequestDto();
        public GetPrivateCourseDto registeredCourse { get; set; } = new GetPrivateCourseDto();
        public List<GetPrivateClassWithNameDto> registeredClasses { get; set; } = new List<GetPrivateClassWithNameDto>();
    }
}