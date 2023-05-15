using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.TeacherPrivateClassDtos
{
    public class GetTeacherPrivateClassWithNameDto
    {
        public int id { get; set; }
        public int teacherId { get; set; }
        public string fullName { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
        public string workType { get; set; } = string.Empty;
        public TeacherClassStatus status { get; set; } = TeacherClassStatus.Incomplete;
    }
}