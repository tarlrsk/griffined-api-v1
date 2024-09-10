namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class StudentAddingConflictResponseDto
    {
        public bool IsConflict { get; set; }
        public List<ConflictScheduleResponseDto> ConflictMessages { get; set; } = new List<ConflictScheduleResponseDto>();
    }
}