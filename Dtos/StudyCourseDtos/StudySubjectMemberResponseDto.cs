using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StudyCourseService
{
    public class StudySubjectMemberResponseDto
    {
        public List<StudentStudySubjectMemberResponseDto> Students { get; set; } = new();
        public List<TeacherStudySubjectMemberResponseDto> Teachers { get; set; } = new();
    }
}