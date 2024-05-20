using griffined_api.Dtos.ScheduleDtos;
using Newtonsoft.Json;

namespace griffined_api.Dtos.AppointentDtos
{
    public class CreateAppointmentDTO
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("availableSchedules")]
        public IEnumerable<AvailableAppointmentScheduleDTO> AvailableAppointmentSchedules { get; set; }
    }
}