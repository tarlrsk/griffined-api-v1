using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/oa")]
    public class OAController : ControllerBase
    {
        public readonly IOAService _oaService;
        public OAController(IOAService oaService)
        {
            _oaService = oaService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetOADto>>>> Get()
        {
            return Ok(await _oaService.GetOA());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetOADto>>>> GetOAById(int id)
        {
            var response = await _oaService.GetOAById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetOADto>>>> AddEA(AddOADto newOA)
        {
            return Ok(await _oaService.AddOA(newOA));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetOADto>>>> UpdateEA(UpdateOADto updatedOA)
        {
            var response = await _oaService.UpdateOA(updatedOA);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetOADto>>> DeleteOAById(int id)
        {
            var response = await _oaService.DeleteOA(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
    }
}