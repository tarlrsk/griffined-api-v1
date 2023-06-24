using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace griffined_api.Dtos.TeacherDtos
{
    public class AddTeacherDto
    {
        [Required]
        public string fName { get; set; } = string.Empty;
        [Required]
        public string lName { get; set; } = string.Empty;
        public string fullName { get { return fName + " " + lName; } }
        [Required]
        public string nickname { get; set; } = string.Empty;
        [Required]
        public string phone { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        [Required]
        public string line { get; set; } = string.Empty;
        public List<WorkTimeRequestDto> workTimes { get; set; } = new List<WorkTimeRequestDto>();
        public bool isActive { get; set; } = true;
    }
}