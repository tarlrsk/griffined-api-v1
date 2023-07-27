using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.CommentDtos
{
    public class CommentResponseDto
    {
        public int StaffId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}