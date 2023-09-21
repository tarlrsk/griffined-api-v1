using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using griffined_api.Dtos.AppointentDtos;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/appointment")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost(), Authorize(Roles = "ea, master")]
        public async Task<ActionResult> NewAppointment(NewAppointmentRequestDto newAppointment)
        {
            return Ok(await _appointmentService.AddNewAppointment(newAppointment));
        }

        [HttpGet(), Authorize(Roles = "ea, master")]
        public async Task<ActionResult> ListAllAppointments()
        {
            return Ok(await _appointmentService.ListAllAppointments());
        }
    }
}