namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class RequestedByWithContactResponseDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Line { get; set; }
    }
}