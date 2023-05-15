using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.TeacherPrivateClassDtos
{
    public class AddTeacherPrivateClassDto
    {
        [Required]
        public int teacherId { get; set; }
        [Required]
        public string workType { get; set; } = string.Empty;
        [Required]
        public TeacherClassStatus status { get; set; } = TeacherClassStatus.Incomplete;
    }
}