using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using Newtonsoft.Json;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class LocalScheduleRequestDto
    {
        [Required]
        public string Date { get; set; } = string.Empty;
        [Required]
        public string FromTime { get; set; } = string.Empty;
        [Required]
        public string ToTime { get; set; } = string.Empty;
        [Required]
        public int StudySubjectId { get; set; }
        [Required]
        public int TeacherId { get; set; }
    }
}