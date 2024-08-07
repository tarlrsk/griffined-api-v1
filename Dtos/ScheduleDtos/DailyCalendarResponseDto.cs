using Newtonsoft.Json;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class DailtyCalendarDTO
    {
        [JsonProperty("teacherId")]
        public int? TeacherId { get; set; }

        [JsonProperty("teacherFirstName")]
        public string? TeacherFirstName { get; set; }

        [JsonProperty("teacherLastName")]
        public string? TeacherLastName { get; set; }

        [JsonProperty("teacherNickName")]
        public string? TeacherNickname { get; set; }

        [JsonProperty("ot")]
        public double OT { get; set; }

        [JsonProperty("sp")]
        public double SP { get; set; }

        [JsonProperty("hourSlots")]
        public List<CalendarHalfDTO?> HourSlots { get; set; } = new List<CalendarHalfDTO?>();
    }
}