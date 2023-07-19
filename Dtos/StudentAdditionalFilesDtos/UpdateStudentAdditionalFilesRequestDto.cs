using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentAddtionalFilesDtos
{
    public class UpdateStudentAdditionalFilesRequestDto
    {
        public int? Id { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}