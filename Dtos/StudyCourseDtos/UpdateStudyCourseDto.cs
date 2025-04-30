namespace griffined_api.Dtos.StudyCourseDtos
{
    public class UpdateStudyCourseDto
    {
        public string Section { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual Method Method { get; set; }
    }
}