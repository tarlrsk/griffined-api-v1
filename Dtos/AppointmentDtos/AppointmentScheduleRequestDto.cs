namespace griffined_api.Dtos.AppointentDtos
{
    public class AppointmentScheduleRequestDto
    {
        [Required]
        public string Date { get; set; } = string.Empty;
        [Required]
        public string FromTime { get; set; } = string.Empty;
        [Required]
        public string ToTime { get; set; } = string.Empty;
    }
}