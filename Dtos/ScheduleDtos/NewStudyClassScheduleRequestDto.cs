using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class NewStudyClassScheduleRequestDto
    {
        public int ClassNo { get; set; }
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public int Date { get; set; }
        public int FromTime { get; set; }
        public int ToTime { get; set; }
    }
}