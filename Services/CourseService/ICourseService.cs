namespace griffined_api.Services.CourseService
{
    public interface ICourseService
    {
        Task<ServiceResponse<List<CourseResponseDto>>> ListAllCourseSubjectLevel();
    }
}