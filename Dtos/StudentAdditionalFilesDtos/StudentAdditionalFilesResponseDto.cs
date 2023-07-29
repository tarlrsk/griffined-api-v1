using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentAddtionalFilesDtos
{
    public class StudentAdditionalFilesResponseDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public string URL { get; set; } = string.Empty;
    }
}