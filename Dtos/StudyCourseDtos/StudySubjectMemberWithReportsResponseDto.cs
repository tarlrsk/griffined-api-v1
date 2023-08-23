using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StudySubjectMemberWithReportsResponseDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;
        public string StudentNickname { get; set; } = string.Empty;
        public FilesResponseDto FiftyPercentReport { get; set; } = new();
        public FilesResponseDto HundredPercentReport { get; set; } = new();
        public FilesResponseDto SpecialReport { get; set; } = new();
    }
}