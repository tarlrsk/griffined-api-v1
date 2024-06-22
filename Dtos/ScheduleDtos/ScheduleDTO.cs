using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class CreateScheduleDTO
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }
    }

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

        [JsonProperty("currentSchedules")]
        public IEnumerable<AvailableAppointmentScheduleDTO>? CurrentSchedules { get; set; }
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

    public class CheckAvailableClassScheduleDTO
    {
        [JsonProperty("courseId")]
        public int CourseId { get; set; }

        [JsonProperty("subjectId")]
        public int SubjectId { get; set; }

        [JsonProperty("levelId")]
        public int? LevelId { get; set; }

        [JsonProperty("teacherId")]
        public int TeacherId { get; set; }

        [JsonProperty("dates")]
        public IEnumerable<string> Dates { get; set; }

        [JsonProperty("days")]
        public IEnumerable<string>? Days { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("currentSchedules")]
        public IEnumerable<AvailableClassScheduleDTO>? CurrentSchedules { get; set; }
    }

    public class AvailableClassScheduleDTO
    {
        [JsonProperty("teacher")]
        public TeacherNameResponseDto Teacher { get; set; }
        [JsonProperty("additionalHours")]
        public AdditionalHours AdditionalHours { get; set; }

        [JsonProperty("courseId")]
        public int CourseId { get; set; }

        [JsonProperty("courseName")]
        public string CourseName { get; set; }

        [JsonProperty("subjectId")]
        public int SubjectId { get; set; }

        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }

        [JsonProperty("levelId")]
        public int? LevelId { get; set; }

        [JsonProperty("levelName")]
        public string? LevelName { get; set; }

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

        [JsonProperty("scheduleStatus")]
        public ClassStatus ScheduleStatus { get; set; }
    }

    public class AdditionalHours
    {
        [JsonProperty("type")]
        public TeacherWorkType TeacherWorkType { get; set; }
        [JsonProperty("hours")]
        public double Hours { get; set; }
    }
}
