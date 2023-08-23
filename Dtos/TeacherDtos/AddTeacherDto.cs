using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace griffined_api.Dtos.TeacherDtos
{
    public class AddTeacherDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        [Required]
        public string Nickname { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Line { get; set; } = string.Empty;
        public List<WorkTimeRequestDto> WorkTimes { get; set; } = new List<WorkTimeRequestDto>();
        public bool isActive { get; set; } = true;
    }
}