namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class NewSubjectDto
    {
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public double Hour { get; set; }
    }
}