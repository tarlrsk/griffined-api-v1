using griffined_api.Dtos.StudyCourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StudyCourseService
{
    public interface IStudyCourseService
    {
        Task<ServiceResponse<String>> AddGroupSchedule(GroupScheduleRequestDto newSchedule);
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
        Task<ServiceResponse<List<TodayMobileResponse>>> GetMobileTodayClass(string date);
    }
}