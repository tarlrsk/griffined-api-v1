using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class AddScheduleDto
    {
        public int requestId { get; set; }
        public AddPrivateCourseDto course { get; set; } = new AddPrivateCourseDto();
        public List<AddPrivateClassDto> classes { get; set; } = new List<AddPrivateClassDto>();
    }
}