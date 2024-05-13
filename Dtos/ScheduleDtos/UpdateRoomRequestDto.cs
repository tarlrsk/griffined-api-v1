using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class UpdateRoomRequestDto
    {
        [Required]
        public int ScheduleId { get; set; }
        public string Room { get; set; } = string.Empty;
    }
}