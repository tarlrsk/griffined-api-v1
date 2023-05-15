using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class GetStudentAttendanceDto
    {
        public GetPrivateCourseDto course { get; set; } = new GetPrivateCourseDto();
        public GetPrivateClassWithNameDto cls { get; set; } = new GetPrivateClassWithNameDto();
    }
}