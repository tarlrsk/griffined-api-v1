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

        public async Task<ServiceResponse<string>> AddStudentReport(StudentReportDetailRequestDto detailRequestDto, IFormFile fileToUpload)
        {
            var response = new ServiceResponse<string>();

            var dbMember = await _context.StudySubjectMember
                                    .Include(m => m.StudentReports)
                                    .FirstOrDefaultAsync(m => m.Student.StudentCode == detailRequestDto.StudentCode && m.StudySubjectId == detailRequestDto.StudySubjectId) ?? throw new NotFoundException("No student found.");

            int teacherId = _firebaseService.GetAzureIdWithToken();

            var dbTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId) ?? throw new NotFoundException("No Teacher found.");

            var existingReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == detailRequestDto.Progression);

            if (existingReport != null)
                throw new BadRequestException("Report already existed");

            var studentReport = new StudentReport
            {
                StudySubjectMemberId = dbMember.Id,
                Teacher = dbTeacher,
                DateUpdated = DateTime.Now,
                Progression = detailRequestDto.Progression
            };

            var reportRequestDto = new AddStudentReportRequestDto
            {
                ReportData = fileToUpload
            };

            var reportEntity = _mapper.Map<StudentReport>(reportRequestDto);
            var fileName = fileToUpload.FileName;
            var objectName = $"students/{detailRequestDto.StudentCode}/study-subjects/{detailRequestDto.StudySubjectId}/{detailRequestDto.Progression}/{fileName}";

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

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "successful";
            return response;
        }

        public async Task<ServiceResponse<StudentReportResponseDto>> StudentGetStudentReport(int studyCourseId)
        {
            var response = new ServiceResponse<StudentReportResponseDto>();

            var data = new StudentReportResponseDto
            {
                StudyCourseId = studyCourseId
            };

            var studentId = _firebaseService.GetAzureIdWithToken();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId) ?? throw new NotFoundException("No Student Found.");

            var dbStudySubjectMembers = await _context.StudySubjectMember
                                        .Include(sm => sm.StudySubject)
                                            .ThenInclude(ss => ss.Subject)
                                        .Include(sm => sm.StudySubject)
                                            .ThenInclude(ss => ss.StudyCourse)
                                                .ThenInclude(sc => sc.Course)
                                        .Include(sm => sm.Student)
                                        .Include(sm => sm.StudentReports)
                                            .ThenInclude(sp => sp.Teacher)
                                        .Where(sm => sm.StudySubject.StudyCourse.Id == studyCourseId)
                                        .ToListAsync() ?? throw new NotFoundException("No Members Found.");

            dbStudySubjectMembers = dbStudySubjectMembers
                                    .GroupBy(sm => sm.Student.Id)
                                    .Select(group => group.First())
                                    .ToList();

            foreach (var dbStudySubjectMember in dbStudySubjectMembers)
            {
                var studentDto = new StudentReportWithStudentResponseDto
                {
                    StudentId = dbStudySubjectMember.Student.Id,
                    StudentCode = dbStudySubjectMember.Student.StudentCode,
                    FirstName = dbStudySubjectMember.Student.FirstName,
                    LastName = dbStudySubjectMember.Student.LastName,
                    Nickname = dbStudySubjectMember.Student.Nickname,
                    Subjects = new List<StudySubjectReportResponseDto>()
                };

                var dbStudySubjects = await _context.StudySubjects
                                        .Include(ss => ss.StudyCourse)
                                            .ThenInclude(sc => sc.Course)
                                        .Include(ss => ss.Subject)
                                        .Include(ss => ss.StudySubjectMember)
                                            .ThenInclude(sm => sm.Student)
                                        .Include(ss => ss.StudySubjectMember)
                                            .ThenInclude(sm => sm.StudentReports)
                                                .ThenInclude(sr => sr.Teacher)
                                        .Where(ss => ss.StudyCourse.Id == studyCourseId && ss.StudySubjectMember.Any(sm => sm.Student.Id == dbStudySubjectMember.Student.Id))
                                        .ToListAsync() ?? throw new NotFoundException("No Study Subject found.");

                foreach (var dbStudySubject in dbStudySubjects)
                {
                    var dbMemberReport = dbStudySubject.StudySubjectMember.FirstOrDefault(sm => sm.Student.Id == dbStudySubjectMember.Student.Id) ?? throw new NotFoundException("No Student Found.");

                    var fiftyPercentReport = dbMemberReport.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.FiftyPercent);
                    var hundredPercentReport = dbMemberReport.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.HundredPercent);
                    var specialReport = dbMemberReport.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.Special);

                    var reportDto = new StudySubjectReportResponseDto
                    {
                        StudySubject = new Dtos.StudyCourseDtos.StudySubjectResponseDto
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject
                        }
                    };

                    if (dbStudySubjectMember.Student.Id == studentId)
                    {
                        reportDto.FiftyPercentReport = fiftyPercentReport != null
                    ? new ReportFileResponseDto
                    {
                        UploadedBy = fiftyPercentReport.Teacher.Id,
                        Progression = Progression.FiftyPercent,
                        File = new FilesResponseDto
                        {
                            FileName = fiftyPercentReport.FileName,
                            ContentType = await _firebaseService.GetContentTypeByObjectName(fiftyPercentReport.ObjectName),
                            URL = await _firebaseService.GetUrlByObjectName(fiftyPercentReport.ObjectName)
                        }
                    }
                    : null;
                        reportDto.HundredPercentReport = hundredPercentReport != null
                        ? new ReportFileResponseDto
                        {
                            UploadedBy = hundredPercentReport.Teacher.Id,
                            Progression = Progression.HundredPercent,
                            File = new FilesResponseDto
                            {
                                FileName = hundredPercentReport.FileName,
                                ContentType = await _firebaseService.GetContentTypeByObjectName(hundredPercentReport.ObjectName),
                                URL = await _firebaseService.GetUrlByObjectName(hundredPercentReport.ObjectName)
                            }
                        }
                        : null;
                        reportDto.SpecialReport = specialReport != null
                        ? new ReportFileResponseDto
                        {
                            UploadedBy = specialReport.Teacher.Id,
                            Progression = Progression.Special,
                            File = new FilesResponseDto
                            {
                                FileName = specialReport.FileName,
                                ContentType = await _firebaseService.GetContentTypeByObjectName(specialReport.ObjectName),
                                URL = await _firebaseService.GetUrlByObjectName(specialReport.ObjectName)
                            }
                        }
                        : null;
                    }
                    else
                    {
                        reportDto.FiftyPercentReport = null;
                        reportDto.HundredPercentReport = null;
                        reportDto.SpecialReport = null;
                    }

                    studentDto.Subjects.Add(reportDto);
                }

                data.Students.Add(studentDto);
            }

            data.Course = dbStudySubjectMembers.First().StudySubject.StudyCourse.Course.course;

            response.Data = data;
            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<StudentReportResponseDto>> TeacherGetStudentReport(int studyCourseId)
        {
            var response = new ServiceResponse<StudentReportResponseDto>();

            var data = new StudentReportResponseDto
            {
                StudyCourseId = studyCourseId
            };

            var dbStudySubjectMembers = await _context.StudySubjectMember
                                        .Include(sm => sm.StudySubject)
                                            .ThenInclude(ss => ss.Subject)
                                        .Include(sm => sm.StudySubject)
                                            .ThenInclude(ss => ss.StudyCourse)
                                                .ThenInclude(sc => sc.Course)
                                        .Include(sm => sm.Student)
                                        .Include(sm => sm.StudentReports)
                                            .ThenInclude(sp => sp.Teacher)
                                        .Where(sm => sm.StudySubject.StudyCourse.Id == studyCourseId)
                                        .ToListAsync() ?? throw new NotFoundException("No Members Found.");

            dbStudySubjectMembers = dbStudySubjectMembers
                                    .GroupBy(sm => sm.Student.Id)
                                    .Select(group => group.First())
                                    .ToList();

            foreach (var dbStudySubjectMember in dbStudySubjectMembers)
            {
                var studentDto = new StudentReportWithStudentResponseDto
                {
                    StudentId = dbStudySubjectMember.Student.Id,
                    StudentCode = dbStudySubjectMember.Student.StudentCode,
                    FirstName = dbStudySubjectMember.Student.FirstName,
                    LastName = dbStudySubjectMember.Student.LastName,
                    Nickname = dbStudySubjectMember.Student.Nickname,
                    Subjects = new List<StudySubjectReportResponseDto>()
                };

                var dbStudySubjects = await _context.StudySubjects
                                        .Include(ss => ss.StudyCourse)
                                            .ThenInclude(sc => sc.Course)
                                        .Include(ss => ss.Subject)
                                        .Include(ss => ss.StudySubjectMember)
                                            .ThenInclude(sm => sm.Student)
                                        .Include(ss => ss.StudySubjectMember)
                                            .ThenInclude(sm => sm.StudentReports)
                                                .ThenInclude(sr => sr.Teacher)
                                        .Where(ss => ss.StudyCourse.Id == studyCourseId && ss.StudySubjectMember.Any(sm => sm.Student.Id == dbStudySubjectMember.Student.Id))
                                        .ToListAsync() ?? throw new NotFoundException("No Study Subject found.");

                foreach (var dbStudySubject in dbStudySubjects)
                {
                    var dbMemberReport = dbStudySubject.StudySubjectMember.FirstOrDefault(sm => sm.Student.Id == dbStudySubjectMember.Student.Id) ?? throw new NotFoundException("No Student Found.");

                    var fiftyPercentReport = dbMemberReport.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.FiftyPercent);
                    var hundredPercentReport = dbMemberReport.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.HundredPercent);
                    var specialReport = dbMemberReport.StudentReports.FirstOrDefault(sr => sr.Progression == Progression.Special);

                    var reportDto = new StudySubjectReportResponseDto
                    {
                        StudySubject = new Dtos.StudyCourseDtos.StudySubjectResponseDto
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject
                        },
                        FiftyPercentReport = fiftyPercentReport != null
                        ? new ReportFileResponseDto
                        {
                            UploadedBy = fiftyPercentReport.Teacher.Id,
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
                            UploadedBy = hundredPercentReport.Teacher.Id,
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
                            UploadedBy = specialReport.Teacher.Id,
                            Progression = Progression.Special,
                            File = new FilesResponseDto
                            {
                                FileName = specialReport.FileName,
                                ContentType = await _firebaseService.GetContentTypeByObjectName(specialReport.ObjectName),
                                URL = await _firebaseService.GetUrlByObjectName(specialReport.ObjectName)
                            }
                        }
                        : null
                    };

                    studentDto.Subjects.Add(reportDto);
                }

                data.Students.Add(studentDto);
            }

            data.Course = dbStudySubjectMembers.First().StudySubject.StudyCourse.Course.course;

            response.Data = data;
            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateStudentReport(StudentReportDetailRequestDto detailRequestDto, IFormFile fileToUpload)
        {
            var response = new ServiceResponse<string>();

            var dbMember = await _context.StudySubjectMember
                                    .Include(m => m.StudentReports)
                                    .FirstOrDefaultAsync(m => m.Student.StudentCode == detailRequestDto.StudentCode && m.StudySubjectId == detailRequestDto.StudySubjectId) ?? throw new NotFoundException("No student found.");

            var teacherId = _firebaseService.GetAzureIdWithToken();
            var dbTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId) ?? throw new NotFoundException("No teacher found.");

            var reportRequestDto = new AddStudentReportRequestDto
            {
                ReportData = fileToUpload
            };

            var reportEntity = _mapper.Map<StudentReport>(reportRequestDto);
            var fileName = fileToUpload.FileName;
            var objectName = $"students/{detailRequestDto.StudentCode}/study-subjects/{detailRequestDto.StudySubjectId}/{detailRequestDto.Progression}/{fileName}";

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
                reportEntity.Progression = detailRequestDto.Progression;
                reportEntity.DateUpdated = DateTime.Now;
                reportEntity.Teacher = dbTeacher;
            }

            var existingReport = dbMember.StudentReports.FirstOrDefault(sr => sr.Progression == detailRequestDto.Progression);

            if (existingReport != null)
            {
                if (fileToUpload.FileName != existingReport.FileName)
                {
                    await _firebaseService.DeleteStorageFileByObjectName(existingReport.ObjectName);
                    _context.StudentReports.Remove(existingReport);

                    dbMember.StudentReports.Add(reportEntity);
                }
                else
                {
                    existingReport.FileName = reportEntity.FileName;
                    existingReport.ObjectName = reportEntity.ObjectName;
                    existingReport.DateUpdated = DateTime.Now;
                    existingReport.Teacher = dbTeacher;
                }
            }
            else
            {
                dbMember.StudentReports.Add(reportEntity);
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "success";
            return response;
        }
    }
}