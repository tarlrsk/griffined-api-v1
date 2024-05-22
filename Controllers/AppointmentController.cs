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

        [HttpPost(), Authorize(Roles = "ea, master, allstaff")]
        public IActionResult NewAppointment(CreateAppointmentDTO request)
        {
            return Ok(_appointmentService.AddNewAppointment(request));
        }

        [HttpGet(), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> ListAllAppointments()
        {
            return Ok(await _appointmentService.ListAllAppointments());
        }

        [HttpGet("{appointmentId}"), Authorize(Roles = "ec, ea, master, allstaff")]
        public async Task<ActionResult> GetAppointmentById(int appointmentId)
        {
            return Ok(await _appointmentService.GetAppointmentById(appointmentId));
        }

        [HttpPut("{appointmentId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> UpdateAppointmentById(int appointmentId, UpdateAppointmentRequestDto updateAppointmentRequestDto)
        {
            return Ok(await _appointmentService.UpdateApoointmentById(appointmentId, updateAppointmentRequestDto));
        }

    }
}