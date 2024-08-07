using griffined_api.Dtos.LevelDtos;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class StaffCoursesDetailResponseDto
    {
        public int StudyCourseId { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public List<StudySubjectResponseDto> Subjects { get; set; } = new();
        public LevelResponseDto Level { get; set; } = new();
        public string Section { get; set; } = string.Empty;
        public Method Method { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public double TotalHour { get; set; }
        public int HourPerDay { get; set; }
        public StudyCourseStatus Status { get; set; }
        public List<ScheduleResponseDto> Schedules { get; set; } = new();
    }
}