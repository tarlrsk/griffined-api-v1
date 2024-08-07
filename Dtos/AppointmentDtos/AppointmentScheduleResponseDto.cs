namespace griffined_api.Dtos.AppointentDtos
{
    public class AppointmentScheduleResponseDto
    {
        public int ScheduleId { get; set; }
        public string Day { get; set; }
        public string Date { get; set; } = string.Empty;
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public double Hour { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public AppointmentSlotStatus ScheduleStatus { get; set; }
    }
}