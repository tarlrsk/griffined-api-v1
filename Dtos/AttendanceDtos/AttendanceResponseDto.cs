using griffined_api.Dtos.StudyCourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AttendanceDtos
{
    public class AttendanceResponseDto
    {
        public int StudyClassId { get; set; }
        public int ClassNo { get; set; }
        public string Section { get; set; } = string.Empty;
        public string? Room { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public StudyCourseStatus CourseStatus { get; set; }
        public StudyCourseType StudyCourseType { get; set; }

        public string Subject { get; set; } = string.Empty;
        public List<StudySubjectResponseDto> AllSubjects { get; set; } = new List<StudySubjectResponseDto>();

        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;

        public List<StudentAttendanceResponseDto> Members { get; set; } = new List<StudentAttendanceResponseDto>();
    }
}