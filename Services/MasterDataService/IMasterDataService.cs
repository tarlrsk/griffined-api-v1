using griffined_api.Dtos.MasterDataDTO;

namespace griffined_api.Services.MasterDataService
{
    public interface IMasterDataService
    {
        /// <summary>
        /// Create new course.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CourseDTO AddCourse(CreateCourseDTO request);
    }
}