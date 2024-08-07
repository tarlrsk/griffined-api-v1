namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class CheckScheduleResultResponseDto
    {
        public bool IsConflict { get; set; } = false;
        public List<AvailableScheduleResponseDto>? AvailableSchedule { get; set; }
        public List<ConflictScheduleResponseDto>? ConflictSchedule { get; set; }
    }
}