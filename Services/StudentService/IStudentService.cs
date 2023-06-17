using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StudentService
{
    public interface IStudentService
    {
        Task<ServiceResponse<List<GetStudentDto>>> GetStudent();
        Task<ServiceResponse<GetStudentDto>> GetStudentById(int id);
        Task<ServiceResponse<GetStudentDto>> GetStudentByToken();
        Task<ServiceResponse<List<GetStudentWithCourseRegisteredCountDto>>> GetStudentWithCoursesRegistered();
        Task<ServiceResponse<GetStudentDto>> AddStudent(AddStudentDto newStudent);
        Task<ServiceResponse<GetStudentDto>> UpdateStudent(UpdateStudentDto updatedStudent);
        Task<ServiceResponse<List<GetStudentDto>>> DeleteStudent(int id);
        Task<ServiceResponse<GetStudentDto>> DisableStudent(int id);
        Task<ServiceResponse<GetStudentDto>> EnableStudent(int id);
        
    }
}