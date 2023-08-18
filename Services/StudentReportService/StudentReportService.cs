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
        private readonly string? FIREBASE_BUCKET = Environment.GetEnvironmentVariable("FIREBASE_BUCKET");
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly StorageClient _storageClient;
        private readonly IFirebaseService _firebaseService;
        public StudentReportService(IMapper mapper, DataContext context, IFirebaseService firebaseService, StorageClient storageClient)
        {
            _mapper = mapper;
            _context = context;
            _firebaseService = firebaseService;
            _storageClient = storageClient;
        }

        public async Task<ServiceResponse<string>> AddStudentReport(int studySubjectId, string studentCode, Progression progression, IFormFile? fileToUpload)
        {
            var response = new ServiceResponse<string>();

            var dbMember = await _context.StudySubjectMember
                                    .Include(m => m.StudentReports)
                                    .FirstOrDefaultAsync(m => m.Student.StudentCode == studentCode && m.StudySubjectId == studySubjectId) ?? throw new NotFoundException("No student found.");

            int teacherId = _firebaseService.GetAzureIdWithToken();

            var existingReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == progression);

            if (existingReport != null)
                throw new BadRequestException("Report already existed");

            if (fileToUpload != null)
            {
                var studentReport = new StudentReport
                {
                    StudySubjectMemberId = dbMember.Id,
                    TeacherId = teacherId,
                    DateUpdated = DateTime.Now,
                    Progression = progression
                };

                var reportRequestDto = new AddStudentReportRequestDto
                {
                    ReportData = fileToUpload
                };

                var reportEntity = _mapper.Map<StudentReport>(reportRequestDto);
                var fileName = fileToUpload.FileName;
                var objectName = $"students/{studentCode}/study subjects/{studySubjectId}/{progression}/{fileName}";

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

        public async Task<ServiceResponse<StudentReportStudentResponseDto>> StudentGetStudentReport(int studyCourseId, string studentCode)
        {
            var response = new ServiceResponse<StudentReportStudentResponseDto>();

            var dbMember = await _context.StudySubjectMember
                                    .Include(m => m.StudySubject)
                                        .ThenInclude(ss => ss.StudyCourse)
                                            .ThenInclude(sc => sc.Course)
                                    .Include(m => m.StudySubject)
                                        .ThenInclude(ss => ss.Subject)
                                    .Include(m => m.StudentReports)
                                    .FirstOrDefaultAsync(m => m.Student.StudentCode == studentCode && m.StudySubject.StudyCourseId == studyCourseId) ?? throw new NotFoundException("No student found.");

            var fiftyPercentReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.FiftyPercent);
            var hundredPercentReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.HundredPercent);
            var specialReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.Special);

            var reportDto = dbMember.StudentReports.Select(async report =>
            new StudySubjectReportResponseDto
            {
                StudySubject = new Dtos.StudyCourseDtos.StudySubjectResponseDto
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

            var data = new StudentReportStudentResponseDto
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

        public async Task<ServiceResponse<StudentReportTeacherResponseDto>> TeacherGetStudentReport(int studyCourseId)
        {
            var response = new ServiceResponse<StudentReportTeacherResponseDto>();

            var dbMembers = await _context.StudySubjectMember
                        .Include(m => m.StudySubject)
                            .ThenInclude(ss => ss.StudyCourse)
                                .ThenInclude(sc => sc.Course)
                        .Include(m => m.StudySubject)
                            .ThenInclude(ss => ss.Subject)
                        .Include(m => m.StudentReports)
                        .Include(m => m.Student)
                        .Where(m => m.StudySubject.StudyCourseId == studyCourseId)
                        .ToListAsync();

            dbMembers = dbMembers
                        .GroupBy(m => m.StudentId)
                        .Select(group => group.First())
                        .ToList();

            var data = new StudentReportTeacherResponseDto
            {
                StudyCourseId = studyCourseId,
                Course = dbMembers.First().StudySubject.StudyCourse.Course.course,
                Students = new List<StudentReportWithStudentResponseDto>()
            };

            foreach (var member in dbMembers)
            {
                var studentReportDto = new StudentReportWithStudentResponseDto
                {
                    StudentId = member.StudentId,
                    StudentCode = member.Student.StudentCode,
                    FirstName = member.Student.FirstName,
                    LastName = member.Student.LastName,
                    Nickname = member.Student.Nickname,
                    Subjects = new List<StudySubjectReportResponseDto>()
                };

                foreach (var subject in member.StudySubject.StudyCourse.StudySubjects)
                {
                    var subjectReportDto = new StudySubjectReportResponseDto
                    {
                        StudySubject = new Dtos.StudyCourseDtos.StudySubjectResponseDto
                        {
                            StudySubjectId = subject.Id,
                            Subject = subject.Subject.subject
                        },
                        FiftyPercentReport = null,
                        HundredPercentReport = null,
                        SpecialReport = null
                    };

                    var fiftyPercentReport = member.StudentReports.FirstOrDefault(report =>
                        report.Progression == Progression.FiftyPercent && report.StudySubjectMember.StudySubjectId == subject.Id);

                    var hundredPercentReport = member.StudentReports.FirstOrDefault(report =>
                        report.Progression == Progression.HundredPercent && report.StudySubjectMember.StudySubjectId == subject.Id);

                    var specialReport = member.StudentReports.FirstOrDefault(report =>
                        report.Progression == Progression.Special && report.StudySubjectMember.StudySubjectId == subject.Id);

                    if (fiftyPercentReport != null)
                    {
                        subjectReportDto.FiftyPercentReport = new ReportFileResponseDto
                        {
                            Progression = Progression.FiftyPercent,
                            File = new FilesResponseDto
                            {
                                FileName = fiftyPercentReport.FileName,
                                ContentType = await _firebaseService.GetContentTypeByObjectName(fiftyPercentReport.ObjectName),
                                URL = await _firebaseService.GetUrlByObjectName(fiftyPercentReport.ObjectName)
                            }
                        };
                    }

                    if (hundredPercentReport != null)
                    {
                        subjectReportDto.HundredPercentReport = new ReportFileResponseDto
                        {
                            Progression = Progression.HundredPercent,
                            File = new FilesResponseDto
                            {
                                FileName = hundredPercentReport.FileName,
                                ContentType = await _firebaseService.GetContentTypeByObjectName(hundredPercentReport.ObjectName),
                                URL = await _firebaseService.GetUrlByObjectName(hundredPercentReport.ObjectName)
                            }
                        };
                    }

                    if (specialReport != null)
                    {
                        subjectReportDto.SpecialReport = new ReportFileResponseDto
                        {
                            Progression = Progression.Special,
                            File = new FilesResponseDto
                            {
                                FileName = specialReport.FileName,
                                ContentType = await _firebaseService.GetContentTypeByObjectName(specialReport.ObjectName),
                                URL = await _firebaseService.GetUrlByObjectName(specialReport.ObjectName)
                            }
                        };
                    }

                    studentReportDto.Subjects.Add(subjectReportDto);
                }

                data.Students.Add(studentReportDto);
            }

            response.Data = data;
            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateStudentReport(int studySubjectId, string studentCode, Progression progression, IFormFile? fileToUpload)
        {
            var response = new ServiceResponse<string>();

            var dbMember = await _context.StudySubjectMember
                                    .Include(m => m.StudentReports)
                                    .FirstOrDefaultAsync(m => m.Student.StudentCode == studentCode && m.StudySubjectId == studySubjectId);

            if (dbMember == null)
                throw new NotFoundException("No student found.");

            if (dbMember.StudentReports != null)
            {
                if (fileToUpload != null)
                {
                    var reportRequestDto = new AddStudentReportRequestDto
                    {
                        ReportData = fileToUpload
                    };

                    var reportEntity = _mapper.Map<StudentReport>(reportRequestDto);
                    var fileName = fileToUpload.FileName;
                    var objectName = $"students/{studentCode}/study subjects/{studySubjectId}/{progression}/{fileName}";

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

                    var existingReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == progression);

                    if (existingReport != null)
                    {
                        if (fileToUpload.FileName != existingReport.FileName)
                        {
                            await _firebaseService.DeleteStorageFileByObjectName(existingReport.ObjectName);
                            _context.StudentReports.Remove(existingReport);

                            dbMember.StudentReports.Add(reportEntity);
                            existingReport.DateUpdated = DateTime.Now;
                        }
                        else
                        {
                            existingReport.FileName = reportEntity.FileName;
                            existingReport.ObjectName = reportEntity.ObjectName;
                            existingReport.DateUpdated = DateTime.Now;
                        }
                    }
                    else
                    {
                        dbMember.StudentReports.Add(reportEntity);
                    }
                }
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "success";
            return response;
        }
    }
}