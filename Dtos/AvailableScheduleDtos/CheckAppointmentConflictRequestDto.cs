namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class CheckAppointmentConflictRequestDto
    {
        public int? AppointmentId { get; set; }
        [Required]
        public List<int?> TeacherIds { get; set; } = new List<int?>();
        [Required]
        public List<LocalAppointmentRequestDto> AppointmentSchedule { get; set; } = new List<LocalAppointmentRequestDto>();
    }
}