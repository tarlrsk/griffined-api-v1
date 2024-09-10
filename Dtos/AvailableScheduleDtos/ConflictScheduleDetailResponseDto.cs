namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictScheduleDetailResponseDto
    {
        public int? ScheduleId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ConflictMemberResponseDto> ConflictMembers { get; set; } = new List<ConflictMemberResponseDto>();
    }
}