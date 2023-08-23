using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentReportDtos
{
    public class ReportFileResponseDto
    {
        public Progression Progression { get; set; }
        public FilesResponseDto File { get; set; } = new FilesResponseDto();
    }
}