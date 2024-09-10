namespace griffined_api.Dtos.AppointentDtos
{
    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public StaffNameOnlyResponseDto CreatedBy { get; set; } = new StaffNameOnlyResponseDto();
        public AppointmentStatus Status { get; set; }
    }
}