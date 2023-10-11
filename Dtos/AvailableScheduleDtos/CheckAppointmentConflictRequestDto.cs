using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class CheckAppointmentConflictRequestDto
    {
        public int? CurrentAppointment { get; set; }
        public List<int> TeacherIds { get; set; } = new List<int>();
        public List<LocalAppointmentRequestDto> AppointmentSchedule { get; set; } = new List<LocalAppointmentRequestDto>();
    }
}