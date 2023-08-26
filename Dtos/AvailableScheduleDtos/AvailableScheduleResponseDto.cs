using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using Newtonsoft.Json;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class AvailableScheduleResponseDto
    {
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int? LevelId { get; set; }
        public string? Level { get; set; }
        public string CourseSubject { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;
    }
}