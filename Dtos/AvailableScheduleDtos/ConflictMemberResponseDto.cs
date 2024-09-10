namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictMemberResponseDto
    {
        public string Role { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
    }
}