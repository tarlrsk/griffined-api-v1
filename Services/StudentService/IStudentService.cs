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
        Task<ServiceResponse<StudentResponseDto>> AddStudent(AddStudentRequestDto newStudent, IFormFile profilePicture, ICollection<IFormFile> files);
        Task<ServiceResponse<StudentResponseDto>> UpdateStudent(UpdateStudentRequestDto updatedStudent, IFormFile profilePicture, ICollection<IFormFile> files);
        Task<ServiceResponse<List<StudentResponseDto>>> DeleteStudent(int id);
        Task<ServiceResponse<StudentResponseDto>> DisableStudent(int id);
        Task<ServiceResponse<StudentResponseDto>> EnableStudent(int id);

    }
}