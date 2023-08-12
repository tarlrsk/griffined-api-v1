using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class ScheduleResponseDto
    {
        public int StudyClassId { get; set; }
        public int ClassNo { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public string CourseSubject { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickName { get; set; } = string.Empty;
        public string? TeacherWorkType { get; set; }
    }
}