using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class CancelledInfoResponseDto
    {
        public int ClassId { get; set; }
        public int ClassNo { get; set; }
        public int StudyCourseId { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int StudySubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string TomTime { get; set; } = string.Empty;
    }
}