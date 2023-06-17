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
        Task<ServiceResponse<GetTeacherDto>> GetTeacherByToken();
        Task<ServiceResponse<GetTeacherDto>> AddTeacher(AddTeacherDto newTeacher);
        Task<ServiceResponse<GetTeacherDto>> UpdateTeacher(UpdateTeacherDto updatedTeacher);
        Task<ServiceResponse<List<GetTeacherDto>>> DeleteTeacher(int id);

        // Update Attendance and Class Status
        Task<ServiceResponse<GetStudentPrivateClassDto>> UpdateStudentAttendance(UpdateStudentPrivateClassDto updatedStudentAttendance);
        Task<ServiceResponse<GetTeacherPrivateClassDto>> UpdateClassStatus(UpdateTeacherPrivateClassDto updatedClassStatus);

        Task<ServiceResponse<GetTeacherDto>> DisableTeacher(int id);
        Task<ServiceResponse<GetTeacherDto>> EnableTeacher(int id);
    }
}