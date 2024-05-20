using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public IEnumerable<DateTime> Dates { get; set; }

        [JsonProperty("days")]
        public IEnumerable<System.DayOfWeek> Days { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("appointmentType")]
        [EnumDataType(typeof(AppointmentType))]
        [JsonConverter(typeof(StringEnumConverter))]
        public AppointmentType AppointmentType { get; set; }
    }

    public class AvailableAppointmentScheduleDTO
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("day")]
        [EnumDataType(typeof(System.DayOfWeek))]
        [JsonConverter(typeof(StringEnumConverter))]
        public System.DayOfWeek Day { get; set; }

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