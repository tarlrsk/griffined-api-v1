using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentReportDtos
{
    public class AddStudentReportRequestDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public required IFormFile ReportData { get; set; }
    }
}