using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.TeacherService
{
    public interface ITeacherService
    {
        // Teacher Services
        Task<ServiceResponse<List<GetTeacherDto>>> GetTeacher();
        Task<ServiceResponse<GetTeacherDto>> GetTeacherById(int id);
        Task<ServiceResponse<GetTeacherDto>> GetTeacherByMe();
        Task<ServiceResponse<List<GetTeacherWithCourseCountDto>>> GetTeacherWithCourseCount();
        Task<ServiceResponse<List<GetTeacherCourseWithClassesDto>>> GetTeacherCourseWithClassesByTeacherId(int teacherId);
        Task<ServiceResponse<List<GetTeacherCourseWithClassesDto>>> GetTeacherCourseWithClassesByMe();
        Task<ServiceResponse<GetStudentAttendanceDto>> GetStudentAttendanceByClassId(int classId);
        Task<ServiceResponse<GetTeacherDto>> AddTeacher(AddTeacherDto newTeacher);
        Task<ServiceResponse<GetTeacherDto>> UpdateTeacher(UpdateTeacherDto updatedTeacher);
        Task<ServiceResponse<List<GetTeacherDto>>> DeleteTeacher(int id);

        // Update Attendance and Class Status
        Task<ServiceResponse<GetStudentPrivateClassDto>> UpdateStudentAttendance(UpdateStudentPrivateClassDto updatedStudentAttendance);
        Task<ServiceResponse<GetTeacherPrivateClassDto>> UpdateClassStatus(UpdateTeacherPrivateClassDto updatedClassStatus);

        Task<ServiceResponse<GetTeacherDto>> DisableTeacher(int id);
        Task<ServiceResponse<GetTeacherDto>> EnableTeacher(int id);

        // Leaving Request Services
        // Task<ServiceResponse<List<GetTeacherLeavingRequestDto>>> GetTeacherLeavingRequest();
        // Task<ServiceResponse<List<GetTeacherLeavingRequestDto>>> GetTeacherLeavingRequestById(int id);
        // Task<ServiceResponse<List<GetTeacherLeavingRequestDto>>> GetTeacherLeavingRequestByTeacherId(int teacherId);
        // Task<ServiceResponse<List<GetTeacherLeavingRequestDto>>> AddTeacherLeavingRequest(AddTeacherLeavingRequestDto newRequest);
        // Task<ServiceResponse<GetTeacherLeavingRequestDto>> UpdateTeacherLeavingRequest(UpdateTeacherLeavingRequestDto updatedRequest);
        // Task<ServiceResponse<List<GetTeacherLeavingRequestDto>>> DeleteTeacherLeavingRequest(int id);
    }
}