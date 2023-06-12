using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/ea")]
    public class EAController : ControllerBase
    {
        public readonly IEAService _eaService;
        public EAController(IEAService eaService)
        {
            _eaService = eaService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetEADto>>>> Get()
        {
            return Ok(await _eaService.GetEA());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetEADto>>>> GetEAById(int id)
        {
            var response = await _eaService.GetEAById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetEADto>>>> AddEA(AddEADto newEA)
        {
            var response = await _eaService.AddEA(newEA);
            if (response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetEADto>>>> UpdateEA(UpdateEADto updatedEA)
        {
            var response = await _eaService.UpdateEA(updatedEA);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetEADto>>> DeleteEAById(int id)
        {
            var response = await _eaService.DeleteEA(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
    }
}