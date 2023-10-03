using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.AppointentDtos
{
    public class AppointmentDetailResponseDto
    {
        public int AppointmentId { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AppointmentStatus Status { get; set; }
        public List<TeacherNameResponseDto> Teachers { get; set; } = new List<TeacherNameResponseDto>();
        public List<AppointmentScheduleResponseDto> Schedules { get; set; } = new List<AppointmentScheduleResponseDto>();
        public List<AppointmentHistoryResponseDto> History { get; set; } = new List<AppointmentHistoryResponseDto>();
    }
}