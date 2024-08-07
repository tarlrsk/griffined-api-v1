namespace griffined_api.Dtos.StaffDtos
{
    public class StaffNameOnlyResponseDto
    {
        public int? StaffId { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? Nickname { get; set; } = string.Empty;

    }
}