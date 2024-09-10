namespace griffined_api.Dtos.ScheduleDtos
{
    public class TodayMobileResponseDto
    {
        public ScheduleType Type { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public TodayMobileClassResponseDto? Class { get; set; }
        public TodayMobileAppointmentResponseDto? Appointment { get; set; }
    }
}