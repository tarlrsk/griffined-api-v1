using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudyCourseHistoryResponseDto
    {
        public DateTime Date { get; set; }
        public StudyCourseHistoryType RecordType { get; set; }
        public string Record { get; set; } = string.Empty;
    }
}