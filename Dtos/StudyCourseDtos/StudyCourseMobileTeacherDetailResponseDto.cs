using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudyCourseMobileTeacherDetailResponseDto
    {
        public int StudyCourseId { get; set; }
        public string Section { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string? Level { get; set; }
        public int StudentCount { get; set; }
        public int TotalHour { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public Method Method { get; set; }
        public List<StudySubjectResponseDto> StudySubjects { get; set; } = new List<StudySubjectResponseDto>();
        public StudyCourseType StudyCourseType { get; set; }
        public StudyCourseStatus CourseStatus { get; set; }
        public List<ScheduleResponseDto> Schedules { get; set; } = new List<ScheduleResponseDto>();
    }
}