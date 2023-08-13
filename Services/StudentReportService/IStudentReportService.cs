using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StudentReportService
{
    public interface IStudentReportService
    {
        Task<ServiceResponse<String>> AddStudentReport(int studySubjectId, string studentCode, Progression progression, IFormFile? fileToUpload);
    }
}