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

        public async Task<ServiceResponse<string>> AddStudentReport(int studySubjectId, string studentCode, Progression progression, IFormFile? fileToUpload)
        {
            var response = new ServiceResponse<string>();

            var dbMember = await _context.StudySubjectMember.FirstOrDefaultAsync(m => m.Student.StudentCode == studentCode && m.StudySubjectId == studySubjectId);

            if (dbMember == null)
                throw new NotFoundException("No student found.");

            int teacherId = _firebaseService.GetAzureIdWithToken();

            var existingReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == progression);

            if (existingReport != null)
                throw new BadRequestException("Report already existed");

            if (fileToUpload != null)
            {
                var studentReport = new StudentReport();

                studentReport.StudySubjectMemberId = dbMember.Id;
                studentReport.TeacherId = teacherId;
                studentReport.DateUpdated = DateTime.Now;
                studentReport.Progression = progression;

                var reportRequestDto = new AddStudentReportRequestDto
                {
                    ReportData = fileToUpload
                };

                var reportEntity = _mapper.Map<StudentReport>(reportRequestDto);
                var fileName = fileToUpload.FileName;
                var objectName = $"students/{studentCode}/{studySubjectId}/{progression}";

                studentReport.FileName = fileName;
                studentReport.ObjectName = objectName;

                using (var stream = reportRequestDto.ReportData.OpenReadStream())
                {
                    var storageObject = await _storageClient.UploadObjectAsync(
                        FIREBASE_BUCKET,
                        objectName,
                        fileToUpload.ContentType,
                        stream
                    );
                }

                dbMember.StudentReports.Add(studentReport);
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "successful";
            return response;
        }

        public async Task<ServiceResponse<StudentReportResponseDto>> StudentGetStudentReport(int studyCourseId, string studentCode)
        {
            var response = new ServiceResponse<StudentReportResponseDto>();

            var dbMember = await _context.StudySubjectMember
                                    .Include(m => m.StudySubject)
                                        .ThenInclude(ss => ss.StudyCourse)
                                            .ThenInclude(sc => sc.Course)
                                    .Include(m => m.StudySubject)
                                        .ThenInclude(ss => ss.Subject)
                                    .Include(m => m.StudentReports)
                                    .FirstOrDefaultAsync(m => m.Student.StudentCode == studentCode && m.StudySubject.StudyCourseId == studyCourseId);

            if (dbMember == null)
                throw new NotFoundException("No student found.");

            var fiftyPercentReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.FiftyPercent);
            var hundredPercentReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.HundredPercent);
            var specialReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.Special);

            var reportDto = dbMember.StudentReports.Select(async report =>
            new StudySubjectReportResponseDto
            {
                StudySubject = new Dtos.ScheduleDtos.StudySubjectResponseDto
                {
                    StudySubjectId = dbMember.StudySubject.Id,
                    Subject = dbMember.StudySubject.Subject.subject
                },
                FiftyPercentReport = fiftyPercentReport != null
                ? new ReportFileResponseDto
                {
                    Progression = Progression.FiftyPercent,
                    File = new FilesResponseDto
                    {
                        FileName = fiftyPercentReport.FileName,
                        ContentType = await _firebaseService.GetContentTypeByObjectName(fiftyPercentReport.ObjectName),
                        URL = await _firebaseService.GetUrlByObjectName(fiftyPercentReport.ObjectName)
                    }
                }
                : null,
                HundredPercentReport = hundredPercentReport != null
                ? new ReportFileResponseDto
                {
                    Progression = Progression.HundredPercent,
                    File = new FilesResponseDto
                    {
                        FileName = hundredPercentReport.FileName,
                        ContentType = await _firebaseService.GetContentTypeByObjectName(hundredPercentReport.ObjectName),
                        URL = await _firebaseService.GetUrlByObjectName(hundredPercentReport.ObjectName)
                    }
                }
                : null,
                SpecialReport = specialReport != null
                ? new ReportFileResponseDto
                {
                    Progression = Progression.Special,
                    File = new FilesResponseDto
                    {
                        FileName = specialReport.FileName,
                        ContentType = await _firebaseService.GetContentTypeByObjectName(specialReport.ObjectName),
                        URL = await _firebaseService.GetUrlByObjectName(specialReport.ObjectName)
                    }
                }
                : null
            }).ToList();

            var reportDtoList = await Task.WhenAll(reportDto);

            var data = new StudentReportResponseDto
            {
                StudyCourseId = studyCourseId,
                course = dbMember.StudySubject.StudyCourse.Course.course,
                StudentCode = studentCode,
                Report = reportDtoList.ToList()
            };

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }
    }
}