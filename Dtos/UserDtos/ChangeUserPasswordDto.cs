namespace griffined_api.Dtos.UserDtos
{
    public class ChangeUserPasswordDto
    {
        public string Password { get; set; } = string.Empty;
        public string VerifyPassword { get; set; } = string.Empty;
    }
}