namespace griffined_api.Dtos.StudentAddtionalFilesDtos
{
    public class AddStudentAdditionalFilesRequestDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public required IFormFile FileData { get; set; }
    }
}