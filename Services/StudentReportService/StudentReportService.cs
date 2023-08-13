using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using griffined_api.Dtos.StudentReportDtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace griffined_api.Services.StudentReportService
{
    public class StudentReportService : IStudentReportService
    {
        private string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private string? FIREBASE_BUCKET = Environment.GetEnvironmentVariable("FIREBASE_BUCKET");
        private string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageClient _storageClient;
        private readonly IFirebaseService _firebaseService;
        public StudentReportService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService, StorageClient storageClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _context = context;
            _firebaseService = firebaseService;
            _storageClient = storageClient;
        }

        public async Task<ServiceResponse<string>> AddStudentReport(int StudySubjectMemberId, string studentCode, Progression progression, IFormFile? fileToUpload)
        {
            var response = new ServiceResponse<string>();

            var dbStudySubject = await _context.StudySubjects
                    .Include(ss => ss.StudySubjectMember)
                        .ThenInclude(m => m.Student)
                    .Include(ss => ss.Subject)
                    .FirstOrDefaultAsync(ss => ss.StudySubjectMember.Any(m => m.Id == StudySubjectMemberId));

            if (dbStudySubject == null)
                throw new NotFoundException("No subject found.");

            var dbMember = dbStudySubject.StudySubjectMember.FirstOrDefault(m => m.Student.StudentCode == studentCode);

            if (dbMember == null)
                throw new NotFoundException("No student found.");

            if (fileToUpload != null)
            {
                var studentReport = new StudentReport();

                studentReport.StudySubjectMemberId = dbMember.Id;
                studentReport.TeacherId = _firebaseService.GetAzureIdWithToken();
                studentReport.DateUpdated = DateTime.Now;
                studentReport.Progression = progression;

                var reportRequestDto = new AddStudentReportRequestDto
                {
                    ReportData = fileToUpload
                };

                var reportEntity = _mapper.Map<StudentReport>(reportRequestDto);
                var fileName = fileToUpload.FileName;
                var objectName = $"students/{studentCode}/{dbStudySubject.Subject.subject}/{progression}";

                using (var stream = reportRequestDto.ReportData.OpenReadStream())
                {
                    var storageObject = await _storageClient.UploadObjectAsync(
                        FIREBASE_BUCKET,
                        objectName,
                        fileToUpload.ContentType,
                        stream
                    );

                    reportEntity.FileName = fileName;
                    reportEntity.ObjectName = objectName;
                }
                string url = await _firebaseService.GetUrlByObjectName(objectName);

                dbMember.StudentReports.Add(studentReport);
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "successful";
            return response;
        }
    }
}