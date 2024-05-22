using griffined_api.Dtos.ScheduleDtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace griffined_api.Dtos.AppointentDtos
{
    public class CreateAppointmentDTO
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("teacherIds")]
        public IEnumerable<int> TeacherIds { get; set; }

        [JsonProperty("appointmentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AppointmentType AppointmentType { get; set; }

        [JsonProperty("schedules")]
        public IEnumerable<CreateScheduleDTO> Schedules { get; set; }
    }
}