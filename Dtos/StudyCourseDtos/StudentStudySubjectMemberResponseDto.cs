using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Services.StudyCourseService
{
    public class StudentStudySubjectMemberResponseDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;
        public string StudentNickname { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CourseJoinedDate { get; set; } = string.Empty;
        public List<StudySubjectResponseDto> Subjects { get; set; } = new();
    }
}