using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class TodayMobileAppointmentResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AppointmentType AppointmentType { get; set; }
    }
}