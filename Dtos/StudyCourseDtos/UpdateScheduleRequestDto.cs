using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Services.StudyCourseService
{
    public class UpdateScheduleRequestDto
    {
        [Required]
        public List<int> StudySubjectIds { get; set; } = new List<int>();
        [Required]
        public List<int> RemoveSchedule { get; set; } = new List<int>();
        [Required]
        public List<ScheduleRequestDto> NewSchedule { get; set; } = new List<ScheduleRequestDto>();
    }
}