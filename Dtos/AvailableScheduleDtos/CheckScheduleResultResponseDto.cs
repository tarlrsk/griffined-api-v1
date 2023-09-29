using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using griffined_api.Dtos.StudyCourseDtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;


namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class CheckScheduleResultResponseDto
    {
        public bool IsConflict { get; set; } = false;
        public List<AvailableScheduleResponseDto>? AvailableSchedule { get; set; }
        public List<ConflictScheduleResponseDto>? ConflictSchedule { get; set; }
    }
}