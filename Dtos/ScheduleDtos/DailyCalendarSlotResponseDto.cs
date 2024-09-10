using Newtonsoft.Json;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class CalendarSlotDTO
    {
        [JsonProperty("scheduleId")]
        public int ScheduleId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("room")]
        public string? Room { get; set; }

        [JsonProperty("type")]
        public DailyCalendarType? Type { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; } = string.Empty;
    }
}