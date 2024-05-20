using Newtonsoft.Json;

namespace griffined_api.Dtos.WorkTimeDtos
{
    public class MandayRequestDto
    {
        [Required]
        [JsonProperty("year")]
        public int Year { get; set; }

        [Required]
        [JsonProperty("workDays")]
        public IEnumerable<WorkTimeRequestDto> WorkDays { get; set; } = new List<WorkTimeRequestDto>();
    }

    public class WorkTimeRequestDto
    {
        [Required]
        [JsonProperty("day")]
        public System.DayOfWeek Day { get; set; }

        [Required]
        [JsonProperty("quarter")]
        public int Quarter { get; set; }

        [Required]
        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [Required]
        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }
    }
}