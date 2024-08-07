namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class RejectedClassCancellationRequestDto
    {
        [Required]
        public string RejectedReason { get; set; } = string.Empty;
    }
}