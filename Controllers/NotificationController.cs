using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet("teacher"), Authorize(Roles = "teacher, master")]
        public async Task<ActionResult> GetTeacherNotifications()
        {
            var response = await _notificationService.GetTeacherNotifications();
            if (response == null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("staff"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetStaffNotifications()
        {
            var response = await _notificationService.GetStaffNotifications();
            if (response == null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("{notificationId}/mark-as-read"), Authorize(Roles = "student, teacher, ec, ea, oa, master")]
        public async Task<ActionResult> MarkAsRead(int notificationId)
        {
            var response = await _notificationService.MarkAsRead(notificationId);
            return Ok(response);
        }

        [HttpPut("/mark-all-as-read"), Authorize(Roles = "student, teacher, ec, ea, oa, master")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var response = await _notificationService.MarkAllAsRead();
            return Ok(response);
        }
    }
}