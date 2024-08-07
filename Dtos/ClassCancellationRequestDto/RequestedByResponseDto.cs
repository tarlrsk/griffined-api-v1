namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class RequestedByResponseDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
    }
}