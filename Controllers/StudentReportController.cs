using griffined_api.Services.StudentReportService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Rpc;
using griffined_api.Dtos.StudentReportDtos;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/student-report")]
    public class StudentReportController : ControllerBase
    {
        private readonly IStudentReportService _studentReportService;
        public StudentReportController(IStudentReportService studentReportService)
        {
            _studentReportService = studentReportService;
        }

        [HttpGet("student"), Authorize(Roles = "teacher, master")]
        public async Task<ActionResult> StudentGetStudentReport(int studyCourseId, string studentCode)
        {
            var response = await _studentReportService.StudentGetStudentReport(studyCourseId, studentCode);
            if (response == null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("teacher")]
        public async Task<ActionResult> TeacherGetStudentReport(int studyCourseId)
        {
            var response = await _studentReportService.TeacherGetStudentReport(studyCourseId);
            if (response == null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost, Authorize(Roles = "teacher, master")]
        public async Task<ActionResult> AddStudentReport([FromForm] StudentReportDetailRequestDto detailRequestDto, IFormFile fileToUpload)
        {
            var response = await _studentReportService.AddStudentReport(detailRequestDto, fileToUpload);
            return Ok(response);
        }

        [HttpPut, Authorize(Roles = "teacher, master")]
        public async Task<ActionResult> UpdateStudentReport([FromForm] StudentReportDetailRequestDto detailRequestDto, IFormFile fileToUpload)
        {
            var response = await _studentReportService.UpdateStudentReport(detailRequestDto, fileToUpload);
            return Ok(response);
        }
    }
}