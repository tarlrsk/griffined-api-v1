namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class StudentAddingConflictRequestDto
    {
        [Required]
        public List<int?> StudySubjectIds { get; set; } = new List<int?>();
        [Required]
        public List<int> StudentIds { get; set; } = new List<int>();
    }
}