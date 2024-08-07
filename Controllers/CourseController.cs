namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/course")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult> ListAllCourseSubjectLevel()
        {
            return Ok(await _courseService.ListAllCourseSubjectLevel());
        }
    }
}