using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;

namespace griffined_api.Services.RegistrationRequestService
{
    public class RegistrationRequestService : IRegistrationRequestService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RegistrationRequestService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<String>> AddNewRequestedCourses(NewCoursesRequestDto newRequestedCourses)
        {
            // TODO Add Comment on Request
            var response = new ServiceResponse<String>();
            var request = new RegistrationRequest();

            if (newRequestedCourses.memberIds == null || newRequestedCourses.memberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }
            foreach (var memberId in newRequestedCourses.memberIds)
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.id == memberId);
                if (dbStudent == null)
                {
                    throw new NotFoundException($"Student with ID {memberId} not found.");
                }
                var member = new RegistrationRequestMember();
                member.student = dbStudent;
                request.registrationRequestMembers.Add(member);
            }

            foreach (var newPreferredDay in newRequestedCourses.preferredDays)
            {
                var requestedPreferredDay = new PreferredDayRequest();
                requestedPreferredDay.day = newPreferredDay.day;
                requestedPreferredDay.fromTime = newPreferredDay.fromTime;
                requestedPreferredDay.toTime = newPreferredDay.toTime;
                request.preferredDayRequests.Add(requestedPreferredDay);
            }

            if (newRequestedCourses.courses == null || newRequestedCourses.courses.Count == 0)
            {
                throw new BadRequestException("The courses field is required.");
            }

            var requestedCourses = newRequestedCourses.courses.Select(c => c.course).ToList();
            var existedCourses = await _context.Courses
                        .Include(c => c.subjects)
                        .Include(c => c.levels)
                        .Where(c => requestedCourses.Contains(c.course)).ToListAsync();

            foreach (var newRequestedCourse in newRequestedCourses.courses)
            {
                var newRequestedCourseRequest = new NewCourseRequest();
                var course = existedCourses.FirstOrDefault(c => c.course == newRequestedCourse.course);
                if (course == null)
                {
                    var newCourse = new Course();
                    newCourse.course = newRequestedCourse.course;

                    if (newRequestedCourse.subjects == null)
                        throw new BadRequestException("The subjects field is required.");

                    foreach (var subject in newRequestedCourse.subjects)
                    {
                        var newSubject = new Subject();
                        newSubject.subject = subject.subject;
                        newCourse.subjects.Add(newSubject);
                    }

                    if (newRequestedCourse.level != null)
                    {
                        var newLevel = new Level();
                        newLevel.level = newRequestedCourse.level;
                        newCourse.levels.Add(newLevel);
                    }

                    _context.Courses.Add(newCourse);
                    await _context.SaveChangesAsync();
                    existedCourses = await _context.Courses
                        .Include(c => c.subjects)
                        .Where(c => requestedCourses.Contains(c.course)).ToListAsync();

                    course = existedCourses.First(c => c.course == newRequestedCourse.course);

                    var level = course.levels.FirstOrDefault(c => c.level == newRequestedCourse.level);
                    newRequestedCourseRequest.level = level;

                    var requestedSubjects = newRequestedCourse.subjects.Select(s => s.subject).ToList();
                    var existedSubjects = course.subjects.Where(s => requestedSubjects.Contains(s.subject));
                    foreach (var requestedSubject in newRequestedCourse.subjects)
                    {
                        var subject = existedSubjects.First(s => s.subject == requestedSubject.subject);
                        var newRequestedSubject = new NewCourseSubjectRequest();
                        newRequestedSubject.subject = subject;
                        newRequestedSubject.hour = requestedSubject.hour;
                        newRequestedCourseRequest.newCourseSubjectRequests.Add(newRequestedSubject);
                    }
                }
                else
                {
                    if (newRequestedCourse.subjects == null)
                        throw new BadRequestException($"The subjects field is required for {newRequestedCourse.course}");
                    foreach (var requestedSubject in newRequestedCourse.subjects)
                    {
                        var newRequestedSubject = new NewCourseSubjectRequest();
                        var subject = course.subjects.FirstOrDefault(s => s.subject == requestedSubject.subject);
                        if (subject == null)
                        {
                            var newSubject = new Subject();
                            newSubject.subject = requestedSubject.subject;
                            newSubject.course = course;
                            _context.Subjects.Add(newSubject);
                            await _context.SaveChangesAsync();
                            existedCourses = await _context.Courses
                                                .Include(c => c.subjects)
                                                .Where(c => requestedCourses
                                                .Contains(c.course)).ToListAsync();
                            course = existedCourses.First(c => c.course == course.course);

                            newRequestedSubject.subject = newSubject;
                        }
                        else
                        {
                            newRequestedSubject.subject = subject;
                        }
                        newRequestedSubject.hour = requestedSubject.hour;
                        newRequestedCourseRequest.newCourseSubjectRequests.Add(newRequestedSubject);
                    }

                    var level = course.levels.FirstOrDefault(c => c.level == newRequestedCourse.level);
                    if (newRequestedCourse.level != null && level != null)
                    {
                        var newLevel = new Level();
                        newLevel.level = newRequestedCourse.level;
                        newLevel.course = course;
                        _context.Levels.Add(newLevel);
                        await _context.SaveChangesAsync();
                        course = await _context.Courses
                                            .Include(c => c.subjects)
                                            .Include(c => c.levels)
                                            .FirstAsync(c => c.course == newRequestedCourse.course);
                        level = course.levels.FirstOrDefault(c => c.level == newRequestedCourse.level);
                    }
                    newRequestedCourseRequest.level = level;
                }

                newRequestedCourseRequest.course = course;
                newRequestedCourseRequest.totalHours = newRequestedCourse.totalHours;
                newRequestedCourseRequest.method = newRequestedCourseRequest.method;
                newRequestedCourseRequest.startDate = DateTime.Parse(newRequestedCourse.startDate);
                newRequestedCourseRequest.startDate = DateTime.Parse(newRequestedCourse.endDate);
                request.newCourseRequests.Add(newRequestedCourseRequest);
            }
            int byECId = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            request.byECId = byECId;
            request.section = newRequestedCourses.sectionName;
            request.type = nameof(RegistrationRequestType.NewRequestedCourse);
            request.registrationStatus = RegistrationStatus.PendingEA;
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<ServiceResponse<String>> AddStudentAddingRequest(StudyAddingRequestDto newRequest)
        {
            var response = new ServiceResponse<String>();
            var request = new RegistrationRequest();

            if (newRequest.memberIds == null || newRequest.memberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }
            foreach (var memberId in newRequest.memberIds)
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.id == memberId);
                if (dbStudent == null)
                {
                    throw new NotFoundException($"Student with ID {memberId} not found.");
                }
                var member = new RegistrationRequestMember();
                member.student = dbStudent;
                request.registrationRequestMembers.Add(member);
            }

            foreach (var studyCourseId in newRequest.courseIds)
            {
                var studyCourse = await _context.StudyCourses.FirstOrDefaultAsync(s => s.id == studyCourseId);
                var newStudentAddingRequest = new StudentAddingRequest();
                if (studyCourse == null)
                {
                    throw new NotFoundException($"Study Course with ID {studyCourseId} not found");
                }
                newStudentAddingRequest.studyCourse = studyCourse;
                request.studentAddingRequest.Add(newStudentAddingRequest);
            }

            int byECId = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            foreach (var comment in newRequest.comments)
            {
                var newComment = new Comment();
                newComment.staffId = byECId;
                request.comments.Add(newComment);
            }

            request.byECId = byECId;
            request.paymentType = newRequest.paymentType;
            request.registrationStatus = RegistrationStatus.PendingEA;
            request.type = nameof(RegistrationRequestType.StudentAdding); //TODO Change request.type to enum if need
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<List<RegistrationRequestResponseDto>>> GetAllRegistrationRequests()
        {
            var response = new ServiceResponse<List<RegistrationRequestResponseDto>>();
            var registrationRequests = await _context.RegistrationRequests
                    .Include(r => r.registrationRequestMembers)
                        .ThenInclude(m => m.student)
                    .ToListAsync();

            var data = new List<RegistrationRequestResponseDto>();

            foreach (var registrationRequest in registrationRequests)
            {
                var staffs = await _context.Staff.ToListAsync();
                var requestDto = new RegistrationRequestResponseDto();

                foreach (var student in registrationRequest.registrationRequestMembers)
                {
                    var studentDto = new StudentNameResponseDto();
                    studentDto.studentId = student.student.id;
                    studentDto.studentCode = student.student.studentId;
                    studentDto.fullName = student.student.fullName;
                    studentDto.nickname = student.student.nickname;
                    requestDto.members.Add(studentDto);
                }

                requestDto.requestId = registrationRequest.id;
                requestDto.type = registrationRequest.type;
                requestDto.registrationStatus = registrationRequest.registrationStatus;
                requestDto.paymentType = registrationRequest.paymentType;
                requestDto.paymentStatus = registrationRequest.paymentStatus;
                requestDto.createdDate = registrationRequest.createdDate;
                requestDto.paymentError = registrationRequest.paymentError;
                requestDto.scheduleError = registrationRequest.scheduleError;
                requestDto.newCourseDetailError = registrationRequest.newCourseDetailError;
                requestDto.hasSchedule = registrationRequest.hasSchedule;

                var ec = staffs.FirstOrDefault(s => s.id == registrationRequest.byECId);
                var ea = staffs.FirstOrDefault(s => s.id == registrationRequest.byEAId);
                var oa = staffs.FirstOrDefault(s => s.id == registrationRequest.byOAId);
                var cancelledBy = staffs.FirstOrDefault(s => s.id == registrationRequest.cancelledBy);

                if (ec != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.nickname = ec.nickname;
                    requestDto.byEC = staff;
                }
                if (ea != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.nickname = ea.nickname;
                    requestDto.byEA = staff;
                }
                if (oa != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.nickname = oa.nickname;
                    requestDto.byEA = staff;
                }
                if (cancelledBy != null)
                {
                    var staff = new StaffNameOnlyResponseDto();
                    staff.nickname = cancelledBy.nickname;
                    requestDto.byEA = staff;
                }

                data.Add(requestDto);

            }
            response.Data = data;
            return response;
        }
    }
}