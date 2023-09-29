using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class AvailableTeacherResponseDto
    {
        public int Id { get; set; }
        public string FirebaseId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public TeacherWorkType TeacherWorkType { get; set; }
    }
}