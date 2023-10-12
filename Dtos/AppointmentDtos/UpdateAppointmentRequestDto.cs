using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.AppointentDtos
{
    public class UpdateAppointmentRequestDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public AppointmentType AppointmentType { get; set; }
        public List<int> ScheduleToDelete { get; set; } = new List<int>();
        public List<AppointmentScheduleRequestDto> ScheduleToAdd { get; set; } = new();
        public List<int> TeacherToDelete { get; set; } = new List<int>();
        public List<int> TeacherToAdd { get; set; } = new List<int>();
    }
}