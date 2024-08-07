namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class AvailableTeacherResponseDto
    {
        public bool IsConflict { get; set; }
        public int TeacherId { get; set; }
        public string FirebaseId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public List<ConflictScheduleResponseDto> ConflictSchedules { get; set; } = new List<ConflictScheduleResponseDto>();
    }
}