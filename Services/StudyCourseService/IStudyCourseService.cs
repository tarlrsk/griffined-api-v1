using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudyCourseDtos;

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
        Task<ServiceResponse<string>> UpdateStudyClassRoom(int studyClassId, string room);
        Task<ServiceResponse<List<StudyCourseByStudentIdResponseDto>>> ListAllStudyCoursesWithReportsByStudentId(int studentId);
    }
}