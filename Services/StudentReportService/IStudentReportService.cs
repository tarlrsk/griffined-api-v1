using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StudentReportService
{
    public interface IStudentReportService
    {
        Task<ServiceResponse<String>> AddStudentReport(string studentCode, string section, string progression, IFormFile? fileToUpload);
    }
}