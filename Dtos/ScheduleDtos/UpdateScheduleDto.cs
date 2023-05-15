using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class UpdateScheduleDto
    {
        public int id { get; set; }
        public UpdatePrivateCourseDto course { get; set; } = new UpdatePrivateCourseDto();
        public List<UpdatePrivateClassDto> classes { get; set; } = new List<UpdatePrivateClassDto>();
    }
}