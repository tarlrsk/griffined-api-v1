using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.AppointentDtos
{
    public class UpdateAppointmentScheduleResponseDto
    {
        public int ScheduleId { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
    }
}