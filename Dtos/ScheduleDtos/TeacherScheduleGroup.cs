using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class TeacherScheduleGroup
    {
        public Teacher? Teacher { get; set; }
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}