using System.Net;
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
        [HttpPost("group"), Authorize(Roles = "ea, master, allstaff")]
        public IActionResult AddGroupSchedule(GroupScheduleRequestDto newRequestedSchedule)
        {
            var studyCourse = _studyCourseService.CreateStudyCourse(newRequestedSchedule);
            var studySubjects = _studyCourseService.CreateStudySubject(studyCourse, newRequestedSchedule);
            _studyCourseService.CreateStudyClass(studyCourse.Id, studySubjects, newRequestedSchedule);
            _studyCourseService.CreateTeacherNotificationForStudySubject(studyCourse.Id);

            return Ok(ResponseWrapper.Success(HttpStatusCode.OK));
        }

        [HttpGet(), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> GetAllStudyCourse()
        {
            return Ok(await _studyCourseService.GetAllStudyCourse());
        }

        [HttpPost("new/{requestId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> AddNewStudyCourses(List<NewStudyClassScheduleRequestDto> newStudyClasses, int requestId)
        {
            return Ok(await _studyCourseService.AddNewStudyClass(newStudyClasses, requestId));
        }

        [HttpPut("new/{requestId}"), Authorize(Roles = "ea, master, allstaff")]
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

        [HttpGet("student/{studyCourseId}"), Authorize(Roles = "student, master, allstaff")]
        public async Task<ActionResult> GetStudyCourseDetailForStudentMobile(int studyCourseId)
        {
            return Ok(await _studyCourseService.StudyCourseDetailForStudent(studyCourseId));
        }

        [HttpGet("teacher/{studyCourseId}"), Authorize(Roles = "teacher, master, allstaff")]
        public async Task<ActionResult> GetStudyCourseDetailForTeacher(int studyCourseId)
        {
            return Ok(await _studyCourseService.StudyCourseDetailForTeacher(studyCourseId));
        }

        [HttpGet("all-courses/student/{studentCode}"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> ListAllStudyCoursesByStudentId(string studentCode)
        {
            return Ok(await _studyCourseService.ListAllStudyCoursesWithReportsByStudentId(studentCode));
        }

        [HttpGet("all-courses/teacher/{teacherId}"), Authorize(Roles = "ea, oa, master, allstaff")]
        public async Task<ActionResult> ListAllStudyCoursesByTeacherId(int teacherId)
        {
            return Ok(await _studyCourseService.ListAllStudyCoursesWithReportsByTeacherId(teacherId));
        }

        [HttpGet("course-detail/{studyCourseId}"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> GetCourseDetail(int studyCourseId)
        {
            return Ok(await _studyCourseService.GetCourseDetail(studyCourseId));
        }

        [HttpGet("course-detail/{studyCourseId}/members"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> GetStudyCourseMember(int studyCourseId)
        {
            return Ok(await _studyCourseService.GetStudyCourseMember(studyCourseId));
        }

        [HttpGet("course-detail/{studyCourseId}/history"), Authorize(Roles = "ec, ea, oa, master, allstaff")]
        public async Task<ActionResult> GetCourseHistory(int studyCourseId)
        {
            return Ok(await _studyCourseService.GetStudyCourseHistory(studyCourseId));
        }

        [HttpPost("add/student"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> EaAddStudent(EaStudentManagementRequestDto requestDto)
        {
            return Ok(await _studyCourseService.EaAddStudent(requestDto));
        }

        [HttpPut("remove/student"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> EaRemoveStudent(EaStudentManagementRequestDto requestDto)
        {
            return Ok(await _studyCourseService.EaRemoveStudent(requestDto));
        }

        [HttpGet("{studyCourseId}/progress"), Authorize(Roles = "ec, ea, oa, teacher, master, allstaff")]
        public async Task<ActionResult> GetSubjectProgress(int studyCourseId)
        {
            return Ok(await _studyCourseService.GetCourseProgress(studyCourseId));
        }

        [HttpPut("schedule"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> UpdateScheduleWithoutRequest(UpdateStudyCourseRequestDto updateRequest)
        {
            return Ok(await _studyCourseService.UpdateScheduleWithoutCancelRequest(updateRequest));
        }

        [HttpPut("cancel/{studyCourseId}"), Authorize(Roles = "ea, master, allstaff")]
        public async Task<ActionResult> CancelStudyCourse(int studyCourseId)
        {
            return Ok(await _studyCourseService.CancelStudyCourse(studyCourseId));
        }
    }
}