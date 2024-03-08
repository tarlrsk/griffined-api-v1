using Newtonsoft.Json;

namespace griffined_api.Dtos.WorkTimeDtos
{
    public class MandayResponseDto
    {
        [Required]
        [JsonProperty("year")]
        public int Year { get; set; }

        [Required]
        [JsonProperty("workDays")]
        public IEnumerable<WorkTimeResponseDto> WorkDays { get; set; } = new List<WorkTimeResponseDto>();
    }

    public class WorkTimeResponseDto
    {
        [Required]
        [JsonProperty("day")]
        public Enums.DayOfWeek Day { get; set; }

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