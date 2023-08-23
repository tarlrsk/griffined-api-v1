using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.LevelDtos
{
    public class LevelResponseDto
    {
        public int LevelId { get; set; }
        public string Level { get; set; } = string.Empty;
    }
}