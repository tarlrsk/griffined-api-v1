using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AttendanceDtos
{
    public class UpdateAttendanceRequestDto
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public Attendance attendance { get; set; }
    }
}