namespace griffined_api.Dtos.StudyCourseDtos
{
    public class EditStudyClassByRegistrationRequestDto
    {
        public List<int> ClassToDelete { get; set; } = new List<int>();
        public List<NewStudyClassScheduleRequestDto> ClassToAdd { get; set; } = new List<NewStudyClassScheduleRequestDto>();
    }
}