using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Services.StudyCourseService
{
    public class TeacherStudySubjectMemberResponseDto
    {
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string JoinDate { get; set; } = string.Empty;
        public List<StudySubjectResponseDto> Subjects { get; set; } = new();
    }
}