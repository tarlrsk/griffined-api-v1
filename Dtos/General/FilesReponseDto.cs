namespace griffined_api.Dtos.General
{
    public class FilesResponseDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public string URL { get; set; } = string.Empty;
    }
}