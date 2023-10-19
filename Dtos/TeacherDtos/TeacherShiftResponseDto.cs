using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class TeacherShiftResponseDto
    {
        public double Hours { get; set; }
        public TeacherWorkType TeacherWorkType { get; set; }
    }
}