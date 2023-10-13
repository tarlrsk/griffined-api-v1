using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.AppointentDtos
{
    public class AppointmentHistoryResponseDto
    {
        public AppointmentHistoryType RecordType { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Record { get; set; } = string.Empty;
    }
}