using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.ScheduleDtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class ScheduleResponseDto
    {
        public int StudyCourseId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public string Day { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int StudySubjectId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string CourseSubject { get; set; } = string.Empty;
        public int StudyClassId { get; set; }
        public int ClassNo { get; set; }
        public string? Room { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public ClassStatus ClassStatus { get; set; }
        public bool IsFiftyPercent { get; set; }
        public bool IsHundredPercent { get; set; }
        public TeacherNameResponseDto Teacher { get; set; }
        public AdditionalHours? AdditionalHours { get; set; }
    }
}