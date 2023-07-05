using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class StudentNameResponseDto
    {
        public int studentId { get; set; }
        public string studentCode { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
    }
}