using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.AppointentDtos
{
    public class UpdateAppointmentRequestDto
    {
        public List<int> ScheduleToDelete { get; set; } = new List<int>();
        public List<AppointmentScheduleRequestDto> ScheduleToAdd { get; set; } = new();
    }
}