using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentPrivateClassDtos
{
    public class GetStudentPrivateClassWithNameDto
    {
        public int id { get; set; }
        public int studentId { get; set; }
        public string fullName { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
        public Attendance attendance { get; set; } = Attendance.None;
    }
}