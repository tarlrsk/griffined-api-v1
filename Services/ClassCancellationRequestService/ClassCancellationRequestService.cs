using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Api;
using griffined_api.Dtos.ClassCancellationRequestDto;
using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Extensions.DateTimeExtensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace griffined_api.Services.ClassCancellationRequestService
{
    public class ClassCancellationRequestService : IClassCancellationRequestService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;

        public ClassCancellationRequestService(DataContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }

        public async Task<ServiceResponse<string>> AddClassCancellationRequest(int studyClassId)
        {
            var dbStudyClass = await _context.StudyClasses
                            .Include(c => c.StudyCourse)
                            .Include(c => c.StudySubject)
                            .FirstOrDefaultAsync(c => c.Id == studyClassId && c.Status == ClassStatus.None)
                            ?? throw new NotFoundException($"StudyClass that can cancel with ID {studyClassId} is not found.");
            
            if(dbStudyClass.StudyCourse.StudyCourseType == StudyCourseType.Group)
                throw new BadRequestException($"Group Class cannot cancel");

            var classCancellationRequest = new ClassCancellationRequest
            {
                RequestedDate = DateTime.Now,
                StudyClass = dbStudyClass,
                StudyCourse = dbStudyClass.StudyCourse,
                StudySubject = dbStudyClass.StudySubject,
            };


            var role = _firebaseService.GetRoleWithToken();
            var userId = _firebaseService.GetAzureIdWithToken();

            if (role == "student")
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == userId)
                                    ?? throw new NotFoundException($"Student with ID {userId} is not found.");

                classCancellationRequest.Student = dbStudent;
                classCancellationRequest.RequestedRole = CancellationRole.Student;
            }
            else
            {
                var dbTeacher = await _context.Teachers.FirstOrDefaultAsync(s => s.Id == userId)
                                    ?? throw new NotFoundException($"Teacher with ID {userId} is not found.");

                classCancellationRequest.Teacher = dbTeacher;
                classCancellationRequest.RequestedRole = CancellationRole.Teacher;
            }

            dbStudyClass.Status = ClassStatus.PendingCancellation;

            _context.ClassCancellationRequests.Add(classCancellationRequest);

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<List<ClassCancellationRequestResponseDto>>> ListAllClassCancellationRequest()
        {
            var dbRequests = await _context.ClassCancellationRequests
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.Subject)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudySubjectMember)
                            .Include(r => r.StudyClass)
                                .ThenInclude(c => c.Schedule)
                            .Include(r => r.Student)
                            .Include(r => r.Teacher)
                            .ToListAsync();


            var data = new List<ClassCancellationRequestResponseDto>();
            foreach (var dbRequest in dbRequests)
            {
                var cancellationRequestDto = new ClassCancellationRequestResponseDto
                {
                    RequestId = dbRequest.Id,
                    StudyCourseId = dbRequest.StudyCourse.Id,
                    Section = dbRequest.StudyCourse.Section,
                    RequestedRole = dbRequest.RequestedRole,
                    Course = dbRequest.StudyCourse.Course.course,
                    Level = dbRequest.StudyCourse.Level?.level,
                    StudyCourseType = dbRequest.StudyCourse.StudyCourseType,
                    RequestedDate = dbRequest.RequestedDate.ToDateTimeString(),
                    CancelledDate = dbRequest.StudyClass.Schedule.Date.ToDateString(),
                    CancelledFromTime = dbRequest.StudyClass.Schedule.FromTime.ToTimeSpanString(),
                    CancelledToTime = dbRequest.StudyClass.Schedule.ToTime.ToTimeSpanString(),
                    Status = dbRequest.Status,
                };

                if (dbRequest.RequestedRole == CancellationRole.Student)
                {
                    if (dbRequest.Student == null)
                        throw new NotFoundException("Student is not found.");

                    foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                    {
                        if (dbStudySubject.StudySubjectMember.Any(m => m.StudentId == dbRequest.StudentId))
                        {
                            cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                            {
                                StudySubjectId = dbStudySubject.Id,
                                SubjectId = dbStudySubject.Subject.Id,
                                Subject = dbStudySubject.Subject.subject,
                            });
                        }
                    }

                    cancellationRequestDto.RequestedBy = new RequestedByResponseDto
                    {
                        UserId = dbRequest.Student.Id,
                        FirstName = dbRequest.Student.FirstName,
                        LastName = dbRequest.Student.LastName,
                        Nickname = dbRequest.Student.Nickname,
                    };
                }
                else
                {
                    if (dbRequest.Teacher == null)
                        throw new NotFoundException("Teacher is not found.");

                    foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                    {
                        cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                        });
                    }

                    cancellationRequestDto.RequestedBy = new RequestedByResponseDto
                    {
                        UserId = dbRequest.Teacher.Id,
                        FirstName = dbRequest.Teacher.FirstName,
                        LastName = dbRequest.Teacher.LastName,
                        Nickname = dbRequest.Teacher.Nickname,
                    };
                }

                if (dbRequest.TakenByEAId != null)
                {
                    var takenByEA = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.TakenByEAId)
                                    ?? throw new InternalServerException("Something went wrong with Taken By Staff");

                    cancellationRequestDto.TakenByEA = new StaffNameOnlyResponseDto
                    {
                        StaffId = takenByEA.Id,
                        FullName = takenByEA.FirstName + " " + takenByEA.LastName,
                        Nickname = takenByEA.Nickname,
                    };
                }

                data.Add(cancellationRequestDto);
            }

            var response = new ServiceResponse<List<ClassCancellationRequestResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
            return response;
        }

        public async Task<ServiceResponse<ClassCancellationRequestDetailResponseDto>> GetClassCancellationRequestDetailByRequestId(int requestId)
        {
            var dbRequest = await _context.ClassCancellationRequests
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.Subject)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudyClasses)
                                        .ThenInclude(c => c.Schedule)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudyClasses)
                                        .ThenInclude(c => c.Teacher)
                            .Include(r => r.StudyCourse)
                                .ThenInclude(c => c.StudySubjects)
                                    .ThenInclude(c => c.StudySubjectMember)
                            .Include(r => r.StudyClass)
                                .ThenInclude(c => c.Schedule)
                            .Include(r => r.Student)
                            .Include(r => r.Teacher)
                            .FirstOrDefaultAsync(r => r.Id == requestId)
                            ?? throw new NotFoundException($"Class Cancellation Request with ID {requestId} is not found.");

            var cancellationRequestDto = new ClassCancellationRequestDetailResponseDto
            {
                RequestId = dbRequest.Id,
                StudyCourseId = dbRequest.StudyCourse.Id,
                Section = dbRequest.StudyCourse.Section,
                RequestedRole = dbRequest.RequestedRole,
                Course = dbRequest.StudyCourse.Course.course,
                Level = dbRequest.StudyCourse.Level?.level,
                StudyCourseType = dbRequest.StudyCourse.StudyCourseType,
                RequestedDate = dbRequest.RequestedDate.ToDateTimeString(),
                CancelledDate = dbRequest.StudyClass.Schedule.Date.ToDateString(),
                CancelledFromTime = dbRequest.StudyClass.Schedule.FromTime.ToTimeSpanString(),
                CancelledToTime = dbRequest.StudyClass.Schedule.ToTime.ToTimeSpanString(),
                Status = dbRequest.Status,
            };

            if (dbRequest.RequestedRole == CancellationRole.Student)
            {
                if (dbRequest.Student == null)
                    throw new NotFoundException("Student is not found.");

                foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                {
                    if (dbStudySubject.StudySubjectMember.Any(m => m.StudentId == dbRequest.StudentId))
                    {
                        cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                        {
                            StudySubjectId = dbStudySubject.Id,
                            SubjectId = dbStudySubject.Subject.Id,
                            Subject = dbStudySubject.Subject.subject,
                        });
                    }
                }

                cancellationRequestDto.RequestedBy = new RequestedByWithContactResponseDto
                {
                    UserId = dbRequest.Student.Id,
                    FirstName = dbRequest.Student.FirstName,
                    LastName = dbRequest.Student.LastName,
                    Nickname = dbRequest.Student.Nickname,
                    Phone = dbRequest.Student.Phone,
                    Email = dbRequest.Student.Email,
                    Line = dbRequest.Student.Line,
                };
            }
            else
            {
                if (dbRequest.Teacher == null)
                    throw new NotFoundException("Teacher is not found.");

                foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
                {
                    cancellationRequestDto.StudySubjects.Add(new StudySubjectResponseDto
                    {
                        StudySubjectId = dbStudySubject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject,
                    });
                }

                cancellationRequestDto.RequestedBy = new RequestedByWithContactResponseDto
                {
                    UserId = dbRequest.Teacher.Id,
                    FirstName = dbRequest.Teacher.FirstName,
                    LastName = dbRequest.Teacher.LastName,
                    Nickname = dbRequest.Teacher.Nickname,
                    Phone = dbRequest.Teacher.Phone,
                    Email = dbRequest.Teacher.Email,
                    Line = dbRequest.Teacher.Line,
                };
            }

            if (dbRequest.TakenByEAId != null)
            {
                var takenByEA = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbRequest.TakenByEAId)
                                ?? throw new InternalServerException("Something went wrong with Taken By Staff");

                cancellationRequestDto.TakenByEA = new StaffNameOnlyResponseDto
                {
                    StaffId = takenByEA.Id,
                    FullName = takenByEA.FirstName + " " + takenByEA.LastName,
                    Nickname = takenByEA.Nickname,
                };
            }

            var rawSchedules = new List<ScheduleResponseDto>();

            foreach (var dbStudySubject in dbRequest.StudyCourse.StudySubjects)
            {
                if (dbRequest.RequestedRole == CancellationRole.Student 
                && dbStudySubject.StudySubjectMember.Any(s => s.StudentId != dbRequest.StudentId))
                {
                    continue;
                }
                foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                {
                    var schedule = new ScheduleResponseDto
                    {
                        StudyCourseId = dbStudySubject.StudyCourse.Id,
                        StudyClassId = dbStudyClass.Id,
                        ClassNo = dbStudyClass.ClassNumber,
                        Room = dbStudyClass.Room,
                        Date = dbStudyClass.Schedule.Date.ToDateString(),
                        FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                        ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                        CourseSubject = dbRequest.StudyCourse.Course.course + " "
                                            + dbStudySubject.Subject.subject
                                            + " " + (dbRequest.StudyCourse.Level?.level ?? ""),
                        CourseId = dbRequest.StudyCourse.Course.Id,
                        Course = dbRequest.StudyCourse.Course.course,
                        StudySubjectId = dbStudySubject.Subject.Id,
                        SubjectId = dbStudySubject.Subject.Id,
                        Subject = dbStudySubject.Subject.subject,
                        TeacherId = dbStudyClass.Teacher.Id,
                        TeacherFirstName = dbStudyClass.Teacher.FirstName,
                        TeacherLastName = dbStudyClass.Teacher.LastName,
                        TeacherNickname = dbStudyClass.Teacher.Nickname,
                        ClassStatus = dbStudyClass.Status,
                    };
                    rawSchedules.Add(schedule);
                }
            }

            cancellationRequestDto.Schedules = rawSchedules.OrderBy(s => (s.Date + " " + s.FromTime).ToDateTime()).ToList();

            var response = new ServiceResponse<ClassCancellationRequestDetailResponseDto>{
                StatusCode = (int)HttpStatusCode.OK,
                Data = cancellationRequestDto,
            };
            return response;
        }

        public async Task<ServiceResponse<string>> EaTakeRequest(int requestId)
        {
            var dbRequest = await _context.ClassCancellationRequests.FirstOrDefaultAsync(c => c.Id == requestId)
                            ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            if(dbRequest.TakenByEAId != null)
                throw new ConflictException($"Request Already Taken By Another EA");

            var eaId = _firebaseService.GetAzureIdWithToken();

            dbRequest.TakenByEAId = eaId;

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>{
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }

        public async Task<ServiceResponse<string>> EaReleaseRequest(int requestId)
        {
            var dbRequest = await _context.ClassCancellationRequests.FirstOrDefaultAsync(c => c.Id == requestId)
                            ?? throw new NotFoundException($"Request with ID {requestId} is not found.");

            if(dbRequest.TakenByEAId == null)
                throw new ConflictException("EA haven't take this request yet.");

            dbRequest.TakenByEAId = null;
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>{
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }
    }
}