using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentAddtionalFilesDtos
{
    public class StudentAdditionalFilesResponseDto
    {
        [Required]
        public string fileName { get; set; } = string.Empty;
    }
}