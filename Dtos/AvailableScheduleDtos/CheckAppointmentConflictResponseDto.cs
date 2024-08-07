namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class CheckAppointmentConflictResponseDto
    {
        public bool IsConflict { get; set; }
        public List<ConflictScheduleResponseDto> ConflictSchedules { get; set; } = new List<ConflictScheduleResponseDto>();
    }
}