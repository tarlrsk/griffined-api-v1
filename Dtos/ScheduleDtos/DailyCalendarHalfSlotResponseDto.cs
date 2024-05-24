using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;
using Newtonsoft.Json;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class CalendarHalfDTO
    {
        [JsonProperty("firstHalf")]
        public CalendarSlotDTO? FirstHalf { get; set; }

        [JsonProperty("secondHalf")]
        public CalendarSlotDTO? SecondHalf { get; set; }
    }
}