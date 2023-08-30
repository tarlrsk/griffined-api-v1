using griffined_api.Dtos.CommentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class RegistrationRequestCommentResponseDto
    {
        public int RequestId { get; set; }
        public List<CommentResponseDto> Comments { get; set; } = new();
    }
}