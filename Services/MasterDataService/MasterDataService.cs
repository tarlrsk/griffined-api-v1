using griffined_api.Dtos.MasterDataDTO;

namespace griffined_api.Services.MasterDataService
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAsyncRepository<Course> _courseRepo;

        public MasterDataService(IUnitOfWork uow,
                                 IAsyncRepository<Course> courseRepo)
        {
            _uow = uow;
            _courseRepo = courseRepo;
        }

        public CourseDTO AddCourse(CreateCourseDTO request)
        {
            var model = new Course
            {
                course = request.Name
            };

            _uow.BeginTran();
            _courseRepo.Add(model);
            _uow.Complete();
            _uow.CommitTran();

            var response = MapCourseToDTO(model);

            return response;
        }

        public CourseDTO MapCourseToDTO(Course model)
        {
            return new CourseDTO
            {
                Id = model.Id
            };
        }
    }
}