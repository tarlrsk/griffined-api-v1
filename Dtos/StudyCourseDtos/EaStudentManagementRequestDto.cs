namespace griffined_api.Dtos.StudyCourseDtos
{
    public class EaStudentManagementRequestDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int StudyCourseId { get; set; }

        [Required]
        public List<int> StudySubjectIds { get; set; } = new();
    }
}