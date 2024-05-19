using griffined_api.Dtos.ScheduleDtos;
using Newtonsoft.Json;

namespace griffined_api.Dtos.AppointentDtos
{
    public class CreateAppointmentDTO
    {
        [JsonProperty("availableSchedules")]
        public IEnumerable<AvailableAppointmentScheduleDTO> AvailableAppointmentSchedules { get; set; }
    }
}