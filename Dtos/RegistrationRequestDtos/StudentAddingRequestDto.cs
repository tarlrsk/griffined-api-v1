namespace griffined_api.Dtos.RegistrationRequestDto
{
    public class StudentAddingRequestDto
    {
        [Required]
        public required List<int> MemberIds { get; set; }
        [Required]
        public PaymentType PaymentType { get; set; }
        [Required]
        public List<StudentAddingCourseRequestDto> StudyCourse { get; set; } = new List<StudentAddingCourseRequestDto>();
        public List<String> Comments { get; set; } = new List<String>();
    }
}