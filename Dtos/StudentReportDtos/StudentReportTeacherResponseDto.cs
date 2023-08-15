using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentReportDtos
{
    public class StudentReportTeacherResponseDto
    {
        public int StudyCourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public List<StudentReportWithStudentResponseDto> Students { get; set; } = new List<StudentReportWithStudentResponseDto>();
    }
}