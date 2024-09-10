namespace griffined_api.Dtos.StaffDtos
{
    public class AddStaffRequestDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Nickname { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Line { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}