using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class AppointmentScheduleDTO : AvailableAppointmentScheduleDTO
    {
        [JsonProperty("scheduleId")]
        public int ScheduleId { get; set; }
    }

    public class CheckAvailableAppointmentScheduleDTO
    {
        [JsonProperty("teacherIds")]
        public IEnumerable<int> TeacherIds { get; set; }

        [JsonProperty("dates")]
        public IEnumerable<string> Dates { get; set; }

        [JsonProperty("days")]
        public IEnumerable<string>? Days { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("appointmentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AppointmentType AppointmentType { get; set; }
    }

    public class AvailableAppointmentScheduleDTO
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("day")]
        [JsonConverter(typeof(StringEnumConverter))]
        public string Day { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("hour")]
        public decimal Hour { get; set; }

        [JsonProperty("accumulatedHour")]
        public decimal AccumulatedHour { get; set; }

        [JsonProperty("scheduleType")]
        public ScheduleType ScheduleType { get; set; }

        [JsonProperty("appointmentType")]
        public AppointmentType? AppointmentType { get; set; }

        [JsonProperty("scheduleStatus")]
        public AppointmentSlotStatus ScheduleStatus { get; set; }
    }
}