using griffined_api.Dtos.LevelDtos;
using griffined_api.Dtos.SubjectDtos;

namespace griffined_api.Dtos.CourseDtos
{
    public class CourseResponseDto
    {
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public List<SubjectResponseDto> Subjects { get; set; } = new List<SubjectResponseDto>();
        public List<LevelResponseDto> Levels { get; set; } = new List<LevelResponseDto>();
    }
}