using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StudentService
{
    public interface IStudentService
    {
        Task<ServiceResponse<List<StudentResponseDto>>> GetStudent();
        Task<ServiceResponse<StudentResponseDto>> GetStudentById(int id);
        Task<ServiceResponse<StudentResponseDto>> GetStudentByToken();
        Task<ServiceResponse<StudentResponseDto>> AddStudent(AddStudentRequestDto newStudent);
        Task<ServiceResponse<StudentResponseDto>> UpdateStudent(UpdateStudentRequestDto updatedStudent);
        Task<ServiceResponse<List<StudentResponseDto>>> DeleteStudent(int id);
        Task<ServiceResponse<StudentResponseDto>> DisableStudent(int id);
        Task<ServiceResponse<StudentResponseDto>> EnableStudent(int id);

    }
}