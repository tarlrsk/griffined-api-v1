using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentAddtionalFilesDtos
{
    public class AddStudentAdditionalFilesRequestDto
    {
        [Required]
        public string fileName { get; set; } = string.Empty;
    }
}