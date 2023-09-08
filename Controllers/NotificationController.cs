using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using griffined_api.Services.NotificationService;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("student"), Authorize(Roles = "student, master")]
        public async Task<ActionResult> GetStudentNotifications()
        {
            var response = await _notificationService.GetStudentNotifications();
            if (response == null)
                return NotFound(response);
            return Ok(response);
        }
    }
}