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