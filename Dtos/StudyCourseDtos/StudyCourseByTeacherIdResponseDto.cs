using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudyCourseByTeacherIdResponseDto
    {
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;

        public int StudyCourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public double TotalHour { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public Method Method { get; set; }
        public List<StudySubjectWithMembersResponseDto> StudySubjects { get; set; } = new();
    }
}