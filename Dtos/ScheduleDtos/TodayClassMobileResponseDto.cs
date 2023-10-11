using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class TodayClassMobileResponseDto
    {
        public int StudyClassId { get; set; }
        public StudyCourseType StudyCourseType { get; set; }
        public int StudyCourseId { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int StudySubjectId { get; set; }
        public int? SubjectId { get; set; }
        public string? Subject { get; set; }
        public int? LevelId { get; set; }
        public string? Level { get; set; }
        public string Section { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public string? Room { get; set; }
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;
    }
}