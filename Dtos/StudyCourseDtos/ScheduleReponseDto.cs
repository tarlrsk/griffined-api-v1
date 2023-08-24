using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class ScheduleResponseDto
    {
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string CourseSubject { get; set; } = string.Empty;
        public int StudyClassId { get; set; }
        public int ClassNo { get; set; }
        public string? Room { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public ClassStatus ClassStatus { get; set; }
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;
        public TeacherWorkType TeacherWorkType { get; set; }
    }
}