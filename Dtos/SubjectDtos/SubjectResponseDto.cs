using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.SubjectDtos
{
    public class SubjectResponseDto
    {
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
    }
}