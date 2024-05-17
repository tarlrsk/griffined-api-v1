using griffined_api.Dtos.MasterDataDTO;
using griffined_api.Services.MasterDataService;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/master")]
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterDataService _masterDataService;

        public MasterDataController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        [HttpPost("courses")]
        public IActionResult AddCourse([FromBody] CreateCourseDTO request)
        {
            var course = _masterDataService.AddCourse(request);

            return StatusCode(201, ResponseWrapper.Success(System.Net.HttpStatusCode.Created, course));
        }
    }
}