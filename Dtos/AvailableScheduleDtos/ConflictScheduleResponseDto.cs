namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictScheduleResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int? StudyCourseId { get; set; }
        public int? AppointmentId { get; set; }
        public List<ConflictMemberResponseDto> ConflictMembers { get; set; } = new List<ConflictMemberResponseDto>();
        public List<ConflictScheduleDetailResponseDto> ConflictScheduleDetail { get; set; } = new List<ConflictScheduleDetailResponseDto>();
    }
}