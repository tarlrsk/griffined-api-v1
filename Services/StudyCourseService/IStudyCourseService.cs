using griffined_api.Dtos.ScheduleDtos;
using griffined_api.Dtos.StudyCourseDtos;

namespace griffined_api.Services.StudyCourseService
{
    public interface IStudyCourseService
    {
        StudyCourse CreateStudyCourse(GroupScheduleRequestDto newSchedule);
        IEnumerable<StudySubject> CreateStudySubject(StudyCourse studyCourse, GroupScheduleRequestDto newSchedule);
        void CreateStudyClass(int studyCourseId, IEnumerable<StudySubject> studySubjects, GroupScheduleRequestDto newRequestedSchedule);
        void CreateTeacherNotificationForStudySubject(int studyCourseId);

        Task<ServiceResponse<List<StudyCourseResponseDto>>> GetAllStudyCourse();
        Task<ServiceResponse<string>> AddNewStudyClass(List<NewStudyClassScheduleRequestDto> newStudyClasses, int requestId);
        Task<ServiceResponse<string>> EditStudyClassByRegisRequest(EditStudyClassByRegistrationRequestDto requestDto, int requestId);
        Task<ServiceResponse<List<StudyCourseMobileResponseDto>>> ListAllStudyCourseByStudentToken();
        Task<ServiceResponse<List<StudyCourseMobileResponseDto>>> ListAllStudyCourseByTeacherToken();
        Task<ServiceResponse<StudyCourseMobileTeacherDetailResponseDto>> StudyCourseDetailForTeacher(int studyCourseId);
        Task<ServiceResponse<StudyCourseMobileStudentDetailResponseDto>> StudyCourseDetailForStudent(int studyCourseId);
        Task<ServiceResponse<string>> UpdateStudyClassRoom(int studyClassId, string room);
        Task<ServiceResponse<List<StudyCourseByStudentIdResponseDto>>> ListAllStudyCoursesWithReportsByStudentId(string studentCode);
        Task<ServiceResponse<List<StudyCourseByTeacherIdResponseDto>>> ListAllStudyCoursesWithReportsByTeacherId(int teacherId);
        Task<ServiceResponse<StaffCoursesDetailResponseDto>> GetCourseDetail(int studyCourseId);
        Task<ServiceResponse<StudySubjectMemberResponseDto>> GetStudyCourseMember(int studyCourseId);
        Task<ServiceResponse<string>> EaAddStudent(EaStudentManagementRequestDto requestDto);
        Task<ServiceResponse<string>> EaRemoveStudent(EaStudentManagementRequestDto requestDto);
        Task<ServiceResponse<string>> UpdateScheduleWithoutCancelRequest(UpdateStudyCourseRequestDto updateRequest);
        Task<ServiceResponse<ClassProgressResponseDto>> GetCourseProgress(int studyCourseId);
        Task<ServiceResponse<List<TodayClassMobileResponseDto>>> GetMobileTodayClass(string date);
        Task<ServiceResponse<List<StudyCourseHistoryResponseDto>>> GetStudyCourseHistory(int studyCourseId);
        Task<ServiceResponse<string>> CancelStudyCourse(int studyCourseId);
    }
}