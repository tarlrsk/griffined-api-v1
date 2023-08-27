using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/study-course")]

    public class StudyCourseController : ControllerBase
    {
        private readonly IStudyCourseService _studyCourseService;
        public StudyCourseController(IStudyCourseService studyCourseService)
        {
            _studyCourseService = studyCourseService;

        }
        [HttpPost("group"), Authorize(Roles = "ec, master")]
        public async Task<ActionResult> AddGroupSchedule(GroupScheduleRequestDto newRequestedSchedule)
        {
            return Ok(await _studyCourseService.AddGroupSchedule(newRequestedSchedule));
        }

        [HttpGet(), Authorize(Roles = "ec, oa, master")]
        public async Task<ActionResult> GetAllStudyCourse()
        {
            return Ok(await _studyCourseService.GetAllStudyCourse());
        }

        [HttpPost("new/{requestId}"), Authorize(Roles = "ea, master")]
        public async Task<ActionResult> AddNewStudyCourses(List<NewStudyClassScheduleRequestDto> newStudyClasses, int requestId)
        {
            return Ok(await _studyCourseService.AddNewStudyClass(newStudyClasses, requestId));
        }

        [HttpPut("new/{requestId}"), Authorize(Roles = "ea, master")]
        public async Task<ActionResult> EditNewStudyCourses(EditStudyClassByRegistrationRequestDto request, int requestId)
        {
            return Ok(await _studyCourseService.EditStudyClassByRegisRequest(request, requestId));
        }

        [HttpGet("student/by-token"), Authorize(Roles = "student")]
        public async Task<ActionResult> ListAllStudyCourseByStudentToken()
        {
            return Ok(await _studyCourseService.ListAllStudyCourseByStudentToken());
        }

        [HttpGet("teacher/by-token"), Authorize(Roles = "teacher")]
        public async Task<ActionResult> ListAllStudyCourseByTeacherToken()
        {
            return Ok(await _studyCourseService.ListAllStudyCourseByTeacherToken());
        }

        [HttpGet("student/{studyCourseId}"), Authorize(Roles = "student, master")]
        public async Task<ActionResult> GetStudyCourseDetailForStudentMobile(int studyCourseId)
        {
            return Ok(await _studyCourseService.StudyCourseDetailForStudent(studyCourseId));
        }

        [HttpGet("teacher/{studyCourseId}"), Authorize(Roles = "teacher, master")]
        public async Task<ActionResult> GetStudyCourseDetailForTeacher(int studyCourseId)
        {
            return Ok(await _studyCourseService.StudyCourseDetailForTeacher(studyCourseId));
        }

        [HttpGet("all-courses/student/{studentCode}"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> ListAllStudyCoursesByStudentId(string studentCode)
        {
            return Ok(await _studyCourseService.ListAllStudyCoursesWithReportsByStudentId(studentCode));
        }

        [HttpGet("all-courses/teacher/{teacherId}"), Authorize(Roles = "ea, oa, master")]
        public async Task<ActionResult> ListAllStudyCoursesByTeacherId(int teacherId)
        {
            return Ok(await _studyCourseService.ListAllStudyCoursesWithReportsByTeacherId(teacherId));
        }

        [HttpGet("course-detail/{studyCourseId}"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetCourseDetail(int studyCourseId)
        {
            return Ok(await _studyCourseService.GetCourseDetail(studyCourseId));
        }

        [HttpGet("course-detail/{studyCourseId}/members"), Authorize(Roles = "ec, ea, oa, master")]
        public async Task<ActionResult> GetStudyCourseMember(int studyCourseId)
        {
            return Ok(await _studyCourseService.GetStudyCourseMember(studyCourseId));
        }

        [HttpPost("add/{studyCourseId}/{studySubjectId}/{studentCode}"), Authorize(Roles = "ea, master")]
        public async Task<ActionResult> EaAddStudent(int studyCourseId, int studySubjectId, string studentCode)
        {
            return Ok(await _studyCourseService.EaAddStudent(studyCourseId, studySubjectId, studentCode));
        }
    }
}