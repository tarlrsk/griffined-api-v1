namespace griffined_api.Dtos.StaffDtos
{
    public class UpdateStaffRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirebaseId { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        [Required]
        public string Nickname { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Line { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}