using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateCourseDtos
{
    public class GetPrivateRegisteredCourseDto
    {
        public List<GetPrivateCourseDto> registeredCourses = new List<GetPrivateCourseDto>();
        public List<GetPreferredDayDto> preferredDays = new List<GetPreferredDayDto>();
    }
}