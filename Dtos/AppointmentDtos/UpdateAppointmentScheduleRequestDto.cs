namespace griffined_api.Dtos.AppointentDtos
{
    public class UpdateAppointmentScheduleResponseDto
    {
        public int ScheduleId { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
    }
}