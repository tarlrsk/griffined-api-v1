using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StudentService
{
    public interface IStudentService
    {
        Task<ServiceResponse<List<StudentResponseDto>>> GetStudent();
        Task<ServiceResponse<StudentResponseDto>> GetStudentByStudentId(string studentCode);
        Task<ServiceResponse<StudentResponseDto>> GetStudentByToken();
        Task<ServiceResponse<StudentResponseDto>> AddStudent(AddStudentRequestDto newStudent, ICollection<IFormFile> files);
        Task<ServiceResponse<StudentResponseDto>> UpdateStudent(UpdateStudentRequestDto updatedStudent);
        Task<ServiceResponse<List<StudentResponseDto>>> DeleteStudent(int studentId);
        Task<ServiceResponse<StudentResponseDto>> DisableStudent(int studentId);
        Task<ServiceResponse<StudentResponseDto>> EnableStudent(int studentId);

    }
}