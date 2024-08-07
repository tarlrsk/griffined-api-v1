using griffined_api.Dtos.CommentDtos;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RegistrationRequestCommentResponseDto
    {
        public int RequestId { get; set; }
        public List<CommentResponseDto> Comments { get; set; } = new();
    }
}