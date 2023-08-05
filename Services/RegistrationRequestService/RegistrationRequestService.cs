using Extensions.DateTimeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.CommentDtos;
using griffined_api.Dtos.RegistrationRequestDto;
using griffined_api.Dtos.ScheduleDtos;
using Google.Cloud.Storage.V1;
using System.Net;

namespace griffined_api.Services.RegistrationRequestService
{
    public class RegistrationRequestService : IRegistrationRequestService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFirebaseService _firebaseService;
        public RegistrationRequestService
        (
            IMapper mapper,
            DataContext context,
            IFirebaseService firebaseService
        )
        {
            _mapper = mapper;
            _context = context;
            _firebaseService = firebaseService;
        }

        public async Task<ServiceResponse<String>> AddNewRequestedCourses(NewCoursesRequestDto newRequestedCourses)
        {
            var response = new ServiceResponse<String>();
            var request = new RegistrationRequest();

            if (newRequestedCourses.MemberIds == null || newRequestedCourses.MemberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }
            foreach (var memberId in newRequestedCourses.MemberIds)
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == memberId);
                if (dbStudent == null)
                {
                    throw new NotFoundException($"Student with ID {memberId} not found.");
                }
                var member = new RegistrationRequestMember();
                member.Student = dbStudent;
                request.RegistrationRequestMembers.Add(member);
            }

            if (newRequestedCourses.Type == StudyCourseType.Private && newRequestedCourses.MemberIds.Count() == 1)
            {
                var student = request.RegistrationRequestMembers.ElementAt(0).Student;
                request.Section = student.Nickname + "/" + student.FirstName;
            }
            else if (newRequestedCourses.Section != null && newRequestedCourses.Section != "" && newRequestedCourses.Type != StudyCourseType.Private)
                request.Section = newRequestedCourses.Section;
            else
                throw new BadRequestException("Bad Request on Section Field, or MemberIds Field, or Type Field");

            foreach (var newPreferredDay in newRequestedCourses.PreferredDays)
            {
                var requestedPreferredDay = new NewCoursePreferredDayRequest();
                requestedPreferredDay.Day = newPreferredDay.Day;
                requestedPreferredDay.FromTime = newPreferredDay.FromTime.ToTimeSpan();
                requestedPreferredDay.ToTime = newPreferredDay.ToTime.ToTimeSpan();
                request.NewCoursePreferredDayRequests.Add(requestedPreferredDay);
            }

            if (newRequestedCourses.Courses == null || newRequestedCourses.Courses.Count == 0)
            {
                throw new BadRequestException("The courses field is required.");
            }

            var requestedCourses = newRequestedCourses.Courses.Select(c => c.Course).ToList();
            var existedCourses = await _context.Courses
                        .Include(c => c.Subjects)
                        .Include(c => c.Levels)
                        .Where(c => requestedCourses.Contains(c.course)).ToListAsync();

            foreach (var newRequestedCourse in newRequestedCourses.Courses)
            {
                var newRequestedCourseRequest = new NewCourseRequest();
                var course = existedCourses.FirstOrDefault(c => c.course == newRequestedCourse.Course);
                if (course == null)
                {
                    var newCourse = new Course();
                    newCourse.course = newRequestedCourse.Course;

                    if (newRequestedCourse.Subjects == null)
                        throw new BadRequestException("The subjects field is required.");

                    foreach (var subject in newRequestedCourse.Subjects)
                    {
                        var newSubject = new Subject();
                        newSubject.subject = subject.Subject;
                        newCourse.Subjects.Add(newSubject);
                    }

                    if (newRequestedCourse.Level != null)
                    {
                        var newLevel = new Level();
                        newLevel.level = newRequestedCourse.Level;
                        newCourse.Levels.Add(newLevel);
                    }

                    _context.Courses.Add(newCourse);
                    await _context.SaveChangesAsync();
                    existedCourses = await _context.Courses
                        .Include(c => c.Subjects)
                        .Where(c => requestedCourses.Contains(c.course)).ToListAsync();

                    course = existedCourses.First(c => c.course == newRequestedCourse.Course);

                    var level = course.Levels.FirstOrDefault(c => c.level == newRequestedCourse.Level);
                    newRequestedCourseRequest.Level = level;

                    var requestedSubjects = newRequestedCourse.Subjects.Select(s => s.Subject).ToList();
                    var existedSubjects = course.Subjects.Where(s => requestedSubjects.Contains(s.subject));
                    foreach (var requestedSubject in newRequestedCourse.Subjects)
                    {
                        var subject = existedSubjects.First(s => s.subject == requestedSubject.Subject);
                        var newRequestedSubject = new NewCourseSubjectRequest();
                        newRequestedSubject.Subject = subject;
                        newRequestedSubject.Hour = requestedSubject.Hour;
                        newRequestedCourseRequest.NewCourseSubjectRequests.Add(newRequestedSubject);
                    }
                }
                else
                {
                    if (newRequestedCourse.Subjects == null)
                        throw new BadRequestException($"The subjects field is required for {newRequestedCourse.Course}");
                    foreach (var requestedSubject in newRequestedCourse.Subjects)
                    {
                        var newRequestedSubject = new NewCourseSubjectRequest();
                        var subject = course.Subjects.FirstOrDefault(s => s.subject == requestedSubject.Subject);
                        if (subject == null)
                        {
                            var newSubject = new Subject();
                            newSubject.subject = requestedSubject.Subject;
                            newSubject.Course = course;
                            _context.Subjects.Add(newSubject);
                            await _context.SaveChangesAsync();
                            existedCourses = await _context.Courses
                                                .Include(c => c.Subjects)
                                                .Where(c => requestedCourses
                                                .Contains(c.course)).ToListAsync();
                            course = existedCourses.First(c => c.course == course.course);

                            newRequestedSubject.Subject = newSubject;
                        }
                        else
                        {
                            newRequestedSubject.Subject = subject;
                        }
                        newRequestedSubject.Hour = requestedSubject.Hour;
                        newRequestedCourseRequest.NewCourseSubjectRequests.Add(newRequestedSubject);
                    }

                    var level = course.Levels.FirstOrDefault(c => c.level == newRequestedCourse.Level);
                    if (newRequestedCourse.Level != null && level != null)
                    {
                        var newLevel = new Level();
                        newLevel.level = newRequestedCourse.Level;
                        newLevel.Course = course;
                        _context.Levels.Add(newLevel);
                        await _context.SaveChangesAsync();
                        course = await _context.Courses
                                            .Include(c => c.Subjects)
                                            .Include(c => c.Levels)
                                            .FirstAsync(c => c.course == newRequestedCourse.Course);
                        level = course.Levels.FirstOrDefault(c => c.level == newRequestedCourse.Level);
                    }
                    newRequestedCourseRequest.Level = level;
                }

                newRequestedCourseRequest.Course = course;
                newRequestedCourseRequest.TotalHours = newRequestedCourse.TotalHours;
                newRequestedCourseRequest.Method = newRequestedCourse.Method;
                newRequestedCourseRequest.StartDate = DateTime.Parse(newRequestedCourse.StartDate);
                newRequestedCourseRequest.EndDate = DateTime.Parse(newRequestedCourse.EndDate);
                request.NewCourseRequests.Add(newRequestedCourseRequest);
            }

            int byECId = _firebaseService.GetAzureIdWithToken();
            request.ByECId = byECId;
            request.Type = RegistrationRequestType.NewRequestedCourse;
            request.RegistrationStatus = RegistrationStatus.PendingEA;

            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == byECId);
            if (staff == null)
            {
                throw new BadRequestException($"Staff with ID {byECId} is not found.");
            }
            foreach (var comment in newRequestedCourses.Comments)
            {
                var newComment = new RegistrationRequestComment();
                newComment.Staff = staff;
                newComment.comment = comment;
                request.RegistrationRequestComments.Add(newComment);
            }
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public async Task<ServiceResponse<string>> AddStudentAddingRequest(StudentAddingRequestDto newRequest, List<IFormFile> newFiles)
        {
            var response = new ServiceResponse<string>();
            var request = new RegistrationRequest();

            if (newRequest.MemberIds == null || newRequest.MemberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }

            var dbStudents = new List<Student>();

            foreach (var memberId in newRequest.MemberIds)
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == memberId);
                if (dbStudent == null)
                {
                    throw new NotFoundException($"Student with ID {memberId} not found.");
                }
                dbStudents.Add(dbStudent);
                var member = new RegistrationRequestMember();
                member.Student = dbStudent;
                request.RegistrationRequestMembers.Add(member);
            }

            foreach (var studyCourse in newRequest.StudyCourse)
            {
                var dbStudyCourse = await _context.StudyCourses
                                                .Include(s => s.StudySubjects)
                                                .FirstOrDefaultAsync(s => s.Id == studyCourse.StudyCourseId);
                var newStudentAddingRequest = new StudentAddingRequest();
                if (dbStudyCourse == null)
                {
                    throw new NotFoundException($"Study Course with ID {studyCourse.StudyCourseId} not found");
                }
                newStudentAddingRequest.StudyCourse = dbStudyCourse;

                foreach (var studySubjectId in studyCourse.StudySubjectIds)
                {
                    var dbStudySubject = dbStudyCourse.StudySubjects.FirstOrDefault(s => s.Id == studySubjectId);
                    if (dbStudySubject == null)
                    {
                        throw new NotFoundException($"Study Subject with ID {studySubjectId} not found");
                    }
                    newStudentAddingRequest.StudentAddingSubjectRequests.Add(new StudentAddingSubjectRequest()
                    {
                        StudySubject = dbStudySubject
                    });

                    foreach (var dbStudent in dbStudents)
                    {
                        dbStudySubject.StudySubjectMember.Add(new StudySubjectMember()
                        {
                            Student = dbStudent,
                            Status = StudySubjectMemberStatus.Pending,
                        });
                    }
                }
                request.StudentAddingRequest.Add(newStudentAddingRequest);
            }

            int byECId = _firebaseService.GetAzureIdWithToken();
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == byECId);
            if (staff == null)
            {
                throw new BadRequestException($"Staff with ID {byECId} is not found.");
            }
            foreach (var comment in newRequest.Comments)
            {
                var newComment = new RegistrationRequestComment();
                newComment.Staff = staff;
                newComment.comment = comment;
                request.RegistrationRequestComments.Add(newComment);
            }

            request.ByECId = byECId;
            request.PaymentType = newRequest.PaymentType;
            request.RegistrationStatus = RegistrationStatus.PendingOA;
            request.Type = RegistrationRequestType.StudentAdding;
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            foreach (var newPaymentFile in newFiles)
            {
                var objectName = await _firebaseService.UploadRegistrationRequestPaymentFile(request.Id, request.DateCreated, newPaymentFile);
                if (!request.RegistrationRequestPaymentFiles.Any(f => f.ObjectName == objectName))
                {
                    request.RegistrationRequestPaymentFiles.Add(new RegistrationRequestPaymentFile()
                    {
                        FileName = newPaymentFile.FileName,
                        ObjectName = objectName,
                    });
                }
            }
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK; ;
            return response;
        }

        public async Task<ServiceResponse<List<RegistrationRequestResponseDto>>> ListRegistrationRequests()
        {
            var response = new ServiceResponse<List<RegistrationRequestResponseDto>>();
            var registrationRequests = await _context.RegistrationRequests
                    .Include(r => r.RegistrationRequestMembers)
                        .ThenInclude(m => m.Student)
                    .ToListAsync();

            var data = new List<RegistrationRequestResponseDto>();

            foreach (var registrationRequest in registrationRequests)
            {
                var staffs = await _context.Staff.ToListAsync();
                var requestDto = new RegistrationRequestResponseDto();

                foreach (var student in registrationRequest.RegistrationRequestMembers)
                {
                    var studentDto = new StudentNameResponseDto();
                    studentDto.StudentId = student.Student.Id;
                    studentDto.StudentCode = student.Student.StudentCode;
                    studentDto.FullName = student.Student.FullName;
                    studentDto.Nickname = student.Student.Nickname;
                    requestDto.Members.Add(studentDto);
                }

                requestDto.RequestId = registrationRequest.Id;
                requestDto.Type = registrationRequest.Type;
                requestDto.RegistrationStatus = registrationRequest.RegistrationStatus;
                requestDto.PaymentType = registrationRequest.PaymentType;
                requestDto.PaymentStatus = registrationRequest.PaymentStatus;
                requestDto.CreatedDate = registrationRequest.DateCreated;
                requestDto.PaymentError = registrationRequest.PaymentError;
                requestDto.ScheduleError = registrationRequest.ScheduleError;
                requestDto.NewCourseDetailError = registrationRequest.NewCourseDetailError;
                requestDto.HasSchedule = registrationRequest.HasSchedule;

                var ec = staffs.FirstOrDefault(s => s.Id == registrationRequest.ByECId);
                var ea = staffs.FirstOrDefault(s => s.Id == registrationRequest.ByEAId);
                var oa = staffs.FirstOrDefault(s => s.Id == registrationRequest.ByOAId);
                var cancelledBy = staffs.FirstOrDefault(s => s.Id == registrationRequest.CancelledBy);

                if (ec != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.Nickname = ec.Nickname;
                    staff.FullName = ec.FullName;
                    requestDto.ByEC = staff;
                }
                if (ea != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.Nickname = ea.Nickname;
                    staff.FullName = ea.FullName;
                    requestDto.ByEA = staff;
                }
                if (oa != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.Nickname = oa.Nickname;
                    staff.FullName = oa.FullName;
                    requestDto.ByEA = staff;
                }
                if (cancelledBy != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.Nickname = cancelledBy.Nickname;
                    staff.FullName = cancelledBy.FullName;
                    requestDto.ByEA = staff;
                }

                data.Add(requestDto);

            }
            response.StatusCode = (int)HttpStatusCode.OK; ;
            response.Data = data;
            return response;
        }
        public async Task<ServiceResponse<RegistrationRequestPendingEADetailResponseDto>> GetPendingEADetail(int requestId)
        {
            var response = new ServiceResponse<RegistrationRequestPendingEADetailResponseDto>();
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.Type == RegistrationRequestType.NewRequestedCourse
                                                && r.RegistrationStatus == RegistrationStatus.PendingEA);

            if (dbRequest == null)
                throw new BadRequestException($"Pending EA Request with ID {requestId} is not found.");

            var requestDetail = new RegistrationRequestPendingEADetailResponseDto();
            requestDetail.RequestId = dbRequest.Id;
            requestDetail.Section = dbRequest.Section;
            requestDetail.RegistrationRequestType = dbRequest.Type;
            requestDetail.RegistrationStatus = dbRequest.RegistrationStatus;

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }

            foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
            {
                var requestedCourse = new RequestedCourseResponseDto()
                {
                    CourseId = dbRequestedCourse.Course.Id,
                    Course = dbRequestedCourse.Course.course,
                    LevelId = dbRequestedCourse.LevelId,
                    Level = dbRequestedCourse.Level?.level,
                    TotalHours = dbRequestedCourse.TotalHours,
                    StartDate = dbRequestedCourse.StartDate.ToString("dd-MMMM-yyyy"),
                    EndDate = dbRequestedCourse.EndDate.ToString("ddd-MMMM-yyyy"),
                    Method = dbRequestedCourse.Method,
                };
                foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                {
                    var requestSubject = new RequestedSubjectResponseDto()
                    {
                        SubjectId = dbRequestSubject.Subject.Id,
                        Subject = dbRequestSubject.Subject.subject,
                        Hour = dbRequestSubject.Hour,
                    };
                    requestedCourse.subjects.Add(requestSubject);
                }
                requestDetail.Courses.Add(requestedCourse);
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId);
                if (staff == null)
                    throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToString("dd-MMMM-yyyy HH:mm:ss");
                comment.Comment = dbComment.comment;
                requestDetail.Comments.Add(comment);
            }

            response.StatusCode = (int)HttpStatusCode.OK; ;
            response.Data = requestDetail;

            return response;
        }
        public async Task<ServiceResponse<RegistrationRequestPendingECResponseDto>> GetPendingECDetail(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Schedule)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.StudyCourse)
                                    .ThenInclude(c => c!.StudySubjects)
                                        .ThenInclude(s => s.StudyClasses)
                                            .ThenInclude(c => c.Teacher)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.NewCoursePreferredDayRequests)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.Type == RegistrationRequestType.NewRequestedCourse
                                                && r.RegistrationStatus == RegistrationStatus.PendingEC);

            if (dbRequest == null)
                throw new BadRequestException($"Pending Payment Request with ID {requestId} is not found.");

            var requestDetail = new RegistrationRequestPendingECResponseDto();
            requestDetail.RequestId = dbRequest.Id;
            requestDetail.Section = dbRequest.Section;
            requestDetail.RegistrationRequestType = dbRequest.Type;
            requestDetail.RegistrationStatus = dbRequest.RegistrationStatus;

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbPreferredDay in dbRequest.NewCoursePreferredDayRequests)
            {
                var preferredDay = new PreferredDayResponseDto()
                {
                    Day = dbPreferredDay.Day,
                    FromTime = dbPreferredDay.FromTime.ToTimeSpanString(),
                    ToTime = dbPreferredDay.ToTime.ToTimeSpanString(),
                };
                requestDetail.PreferredDays.Add(preferredDay);
            }


            var rawSchedules = new List<ScheduleResponseDto>();
            foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
            {
                var requestedCourse = new RequestedCourseResponseDto()
                {
                    CourseId = dbRequestedCourse.Course.Id,
                    Course = dbRequestedCourse.Course.course,
                    LevelId = dbRequestedCourse.LevelId,
                    Level = dbRequestedCourse.Level?.level,
                    TotalHours = dbRequestedCourse.TotalHours,
                    StartDate = dbRequestedCourse.StartDate.ToString("dd-MMMM-yyyy"),
                    EndDate = dbRequestedCourse.EndDate.ToString("ddd-MMMM-yyyy"),
                    Method = dbRequestedCourse.Method,
                };
                foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                {
                    var requestSubject = new RequestedSubjectResponseDto()
                    {
                        SubjectId = dbRequestSubject.Subject.Id,
                        Subject = dbRequestSubject.Subject.subject,
                        Hour = dbRequestSubject.Hour,
                    };
                    requestedCourse.subjects.Add(requestSubject);
                }
                requestDetail.Courses.Add(requestedCourse);

                if (dbRequestedCourse.StudyCourse == null)
                    throw new InternalServerException($"New Course with ID {dbRequestedCourse.Id} does not contain any StudyCourse");

                foreach (var dbStudySubject in dbRequestedCourse.StudyCourse.StudySubjects)
                {
                    foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                    {
                        var schedule = new ScheduleResponseDto()
                        {
                            ClassNo = dbStudyClass.ClassNumber,
                            Date = dbStudyClass.Schedule.Date.ToDateString(),
                            FromTime = dbStudyClass.Schedule.FromTime.ToTimeSpanString(),
                            ToTime = dbStudyClass.Schedule.ToTime.ToTimeSpanString(),
                            CourseSubject = dbRequestedCourse.Course.course + " "
                                            + (dbRequestedCourse.NewCourseSubjectRequests.First(r => r.SubjectId == dbStudySubject.SubjectId)).Subject.subject
                                            + " " + (dbRequestedCourse.Level?.level ?? ""),
                            TeacherId = dbStudyClass.Teacher.Id,
                            TeacherFirstName = dbStudyClass.Teacher.FirstName,
                            TeacherLastName = dbStudyClass.Teacher.LastName,
                            TeacherNickName = dbStudyClass.Teacher.Nickname,
                            //TODO Teacher Work Type
                        };
                        rawSchedules.Add(schedule);
                    }
                }
            }

            requestDetail.Schedules = rawSchedules.OrderBy(s => DateTime.ParseExact(s.Date + s.FromTime, "dd-MMMM-yyyyHH:mm", null)).ToList();

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId);
                if (staff == null)
                    throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToString("dd-MMMM-yyyy HH:mm:ss");
                comment.Comment = dbComment.comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<RegistrationRequestPendingECResponseDto>();
            response.Data = requestDetail;
            response.StatusCode = (int)HttpStatusCode.OK; ;
            return response;
        }

        public async Task<ServiceResponse<String>> SubmitPayment(int requestId, SubmitPaymentRequestDto paymentRequest, List<IFormFile> newFiles)
        {
            var dbRequest = await _context.RegistrationRequests
                                    .Include(r => r.RegistrationRequestPaymentFiles)
                                    .FirstOrDefaultAsync(
                                    r => r.Id == requestId
                                    && r.RegistrationStatus == RegistrationStatus.PendingEC);
            if (dbRequest == null)
                throw new BadRequestException($"Pending EC request with ID {requestId} is not found.");

            var removeFiles = dbRequest.RegistrationRequestPaymentFiles
                                        .Where(f => paymentRequest.removeFileName
                                        .Contains(f.FileName))
                                        .ToList();
            foreach (var file in removeFiles)
            {
                await _firebaseService.DeleteStorageFileByObjectName(file.ObjectName);
                dbRequest.RegistrationRequestPaymentFiles.Remove(file);
            }

            foreach (var newPaymentFile in newFiles)
            {
                var objectName = await _firebaseService.UploadRegistrationRequestPaymentFile(requestId, dbRequest.DateCreated, newPaymentFile);
                if (!dbRequest.RegistrationRequestPaymentFiles.Any(f => f.ObjectName == objectName))
                {
                    dbRequest.RegistrationRequestPaymentFiles.Add(new RegistrationRequestPaymentFile()
                    {
                        FileName = newPaymentFile.FileName,
                        ObjectName = objectName,
                    });
                }
            }
            dbRequest.RegistrationStatus = RegistrationStatus.PendingOA;
            dbRequest.PaymentByECId = _firebaseService.GetAzureIdWithToken();
            await _context.SaveChangesAsync();


            var response = new ServiceResponse<String>();
            response.StatusCode = (int)HttpStatusCode.OK; ;
            return response;
        }

        public async Task<ServiceResponse<RegistrationRequestPendingOAResponseDto>> GetPendingOADetail(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.NewCourseSubjectRequests)
                                    .ThenInclude(s => s.Subject)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Course)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(c => c.Level)
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                            .Include(r => r.RegistrationRequestPaymentFiles)
                            .Include(r => r.RegistrationRequestComments)
                            .FirstOrDefaultAsync(r => r.Id == requestId
                                                && r.RegistrationStatus == RegistrationStatus.PendingOA);

            if (dbRequest == null)
                throw new BadRequestException($"Pending Payment Request with ID {requestId} is not found.");

            var requestDetail = new RegistrationRequestPendingOAResponseDto();
            requestDetail.RequestId = dbRequest.Id;
            requestDetail.Section = dbRequest.Section;
            requestDetail.RegistrationRequestType = dbRequest.Type;
            requestDetail.RegistrationStatus = dbRequest.RegistrationStatus;
            requestDetail.PaymentType = dbRequest.PaymentType;

            foreach (var dbMember in dbRequest.RegistrationRequestMembers)
            {
                var member = new StudentNameResponseDto()
                {
                    StudentId = dbMember.Student.Id,
                    StudentCode = dbMember.Student.StudentCode,
                    FullName = dbMember.Student.FullName,
                    Nickname = dbMember.Student.Nickname,
                };
                requestDetail.Members.Add(member);
            }

            foreach (var dbRequestedCourse in dbRequest.NewCourseRequests)
            {
                var requestedCourse = new RequestedCourseResponseDto()
                {
                    CourseId = dbRequestedCourse.Course.Id,
                    Course = dbRequestedCourse.Course.course,
                    LevelId = dbRequestedCourse.LevelId,
                    Level = dbRequestedCourse.Level?.level,
                    TotalHours = dbRequestedCourse.TotalHours,
                    StartDate = dbRequestedCourse.StartDate.ToDateString(),
                    EndDate = dbRequestedCourse.EndDate.ToDateString(),
                    Method = dbRequestedCourse.Method,
                };
                foreach (var dbRequestSubject in dbRequestedCourse.NewCourseSubjectRequests)
                {
                    var requestSubject = new RequestedSubjectResponseDto()
                    {
                        SubjectId = dbRequestSubject.Subject.Id,
                        Subject = dbRequestSubject.Subject.subject,
                        Hour = dbRequestSubject.Hour,
                    };
                    requestedCourse.subjects.Add(requestSubject);
                }
                requestDetail.Courses.Add(requestedCourse);
            }

            foreach (var dbPaymentFile in dbRequest.RegistrationRequestPaymentFiles)
            {
                var url = await _firebaseService.GetUrlByObjectName(dbPaymentFile.ObjectName);
                var ObjectMetaData = await _firebaseService.GetObjectByObjectName(dbPaymentFile.ObjectName);
                requestDetail.PaymentFiles.Add(new FilesResponseDto
                {
                    FileName = dbPaymentFile.FileName,
                    ContentType = ObjectMetaData.ContentType,
                    URL = url
                });
            }

            foreach (var dbComment in dbRequest.RegistrationRequestComments)
            {
                var comment = new CommentResponseDto();
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == dbComment.StaffId);
                if (staff == null)
                    throw new InternalServerException("Something went wrong on staff comment");

                comment.StaffId = staff.Id;
                comment.Role = staff.Role;
                comment.FullName = staff.FullName;
                comment.CreatedAt = dbComment.DateCreated.ToDateTimeString();
                comment.Comment = dbComment.comment;
                requestDetail.Comments.Add(comment);
            }

            var response = new ServiceResponse<RegistrationRequestPendingOAResponseDto>();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = requestDetail;
            return response;
        }

        public async Task<ServiceResponse<string>> ApprovePayment(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .Include(r => r.RegistrationRequestMembers)
                                .ThenInclude(m => m.Student)
                                    .ThenInclude(s => s.StudySubjectMember)
                            .Include(r => r.NewCourseRequests)
                                .ThenInclude(r => r.StudyCourse)
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingOA) 
                            ?? throw new BadRequestException($"PendingOA Request with ID {requestId} is not found");
                            
            foreach(var member in dbRequest.RegistrationRequestMembers)
            {
                foreach(var studySubjectMember in member.Student.StudySubjectMember)
                {
                    studySubjectMember.Status = StudySubjectMemberStatus.Success;
                }
            }
            
            foreach(var newCourseRequest in dbRequest.NewCourseRequests)
            {
                if(newCourseRequest.StudyCourse != null)
                    newCourseRequest.StudyCourse.Status = CourseStatus.NotStarted;
            }
            dbRequest.PaymentStatus = PaymentStatus.Complete;
            dbRequest.RegistrationStatus = RegistrationStatus.Completed;
            dbRequest.ByOAId = _firebaseService.GetAzureIdWithToken();
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return response;
        }
        
        public async Task<ServiceResponse<string>> DeclinePayment(int requestId)
        {
            var dbRequest = await _context.RegistrationRequests
                            .FirstOrDefaultAsync(r => r.Id == requestId && r.RegistrationStatus == RegistrationStatus.PendingOA)
                            ?? throw new BadRequestException($"PendingOA Request with ID {requestId} is not found");
            dbRequest.PaymentError = true;
            dbRequest.PaymentStatus = PaymentStatus.Incomplete;
            dbRequest.RegistrationStatus = RegistrationStatus.PendingEC;
            dbRequest.ByOAId = _firebaseService.GetAzureIdWithToken();
            await _context.SaveChangesAsync();
            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return response;
        }
    }


}