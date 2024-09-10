namespace griffined_api.Dtos.AppointentDtos
{
    public class NewAppointmentRequestDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public AppointmentType AppointmentType { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public List<int> TeacherIds { get; set; } = new List<int>();
        public List<AppointmentScheduleRequestDto> Schedules { get; set; } = new List<AppointmentScheduleRequestDto>();
    }
}