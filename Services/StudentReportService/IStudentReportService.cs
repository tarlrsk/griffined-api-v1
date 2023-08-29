using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudentReportDtos;

namespace griffined_api.Services.StudentReportService
{
    public interface IStudentReportService
    {
        Task<ServiceResponse<StudentReportResponseDto>> StudentGetStudentReport(int studyCourseId);
        Task<ServiceResponse<StudentReportResponseDto>> TeacherGetStudentReport(int studyCourseId);
        Task<ServiceResponse<String>> AddStudentReport(StudentReportDetailRequestDto detailRequestDto, IFormFile fileToUpload);
        Task<ServiceResponse<String>> UpdateStudentReport(StudentReportDetailRequestDto detailRequestDto, IFormFile fileToUpload);
    }
}