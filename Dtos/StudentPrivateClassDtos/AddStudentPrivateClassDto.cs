using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentPrivateClassDtos
{
    public class AddStudentPrivateClassDto
    {
        public int studentId { get; set; }
        [Required]
        public Attendance attendance { get; set; } = Attendance.None;
    }
}