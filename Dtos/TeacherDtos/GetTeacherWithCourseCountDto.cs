using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.TeacherDtos
{
    public class GetTeacherWithCourseCountDto
    {
        public int id { get; set; }
        public string fullName { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
        public int courseCount { get; set; }
    }
}