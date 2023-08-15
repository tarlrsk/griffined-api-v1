using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.StudentReportDtos;

namespace griffined_api.Services.StudentReportService
{
    public interface IStudentReportService
    {
        Task<ServiceResponse<StudentReportResponseDto>> StudentGetStudentReport(int studyCourseId, string studentCode);
        Task<ServiceResponse<String>> AddStudentReport(int studySubjectId, string studentCode, Progression progression, IFormFile? fileToUpload);
    }
}