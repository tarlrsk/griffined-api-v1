using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/ep")]
    public class EPController : ControllerBase
    {
        public readonly IEPService _epService;
        public EPController(IEPService epService)
        {
            _epService = epService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetEPDto>>>> Get()
        {
            return Ok(await _epService.GetEP());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetEADto>>>> GetEPById(int id)
        {
            var response = await _epService.GetEPById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetEPDto>>>> AddEP(AddEPDto newEP)
        {
            var response = await _epService.AddEP(newEP);
            if (response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetEPDto>>>> UpdateEP(UpdateEPDto updatedEP)
        {
            var response = await _epService.UpdateEP(updatedEP);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetEPDto>>> DeleteEPById(int id)
        {
            var response = await _epService.DeleteEP(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
    }
}