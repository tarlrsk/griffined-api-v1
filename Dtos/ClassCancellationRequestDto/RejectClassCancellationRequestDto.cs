using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ClassCancellationRequestDto
{
    public class RejectedClassCancellationRequestDto
    {
        [Required]
        public string RejectedReason { get; set; } = string.Empty;
    }
}