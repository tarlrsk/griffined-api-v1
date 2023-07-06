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

            foreach (var newPreferredDay in newRequestedCourses.PreferredDays)
            {
                var requestedPreferredDay = new PreferredDayRequest();
                requestedPreferredDay.Day = newPreferredDay.Day;
                requestedPreferredDay.FromTime = newPreferredDay.FromTime;
                requestedPreferredDay.ToTime = newPreferredDay.ToTime;
                request.PreferredDayRequests.Add(requestedPreferredDay);
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
                newRequestedCourseRequest.Method = newRequestedCourseRequest.Method;
                newRequestedCourseRequest.StartDate = DateTime.Parse(newRequestedCourse.StartDate);
                newRequestedCourseRequest.EndDate = DateTime.Parse(newRequestedCourse.EndDate);
                request.NewCourseRequests.Add(newRequestedCourseRequest);
            }
            int byECId = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            request.ByECId = byECId;
            request.Section = newRequestedCourses.SectionName;
            request.Type = nameof(RegistrationRequestType.NewRequestedCourse);
            request.RegistrationStatus = RegistrationStatus.PendingEA;
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<ServiceResponse<String>> AddStudentAddingRequest(StudyAddingRequestDto newRequest)
        {
            var response = new ServiceResponse<String>();
            var request = new RegistrationRequest();

            if (newRequest.MemberIds == null || newRequest.MemberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }
            foreach (var memberId in newRequest.MemberIds)
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

            foreach (var studyCourseId in newRequest.CourseIds)
            {
                var studyCourse = await _context.StudyCourses.FirstOrDefaultAsync(s => s.Id == studyCourseId);
                var newStudentAddingRequest = new StudentAddingRequest();
                if (studyCourse == null)
                {
                    throw new NotFoundException($"Study Course with ID {studyCourseId} not found");
                }
                newStudentAddingRequest.StudyCourse = studyCourse;
                request.StudentAddingRequest.Add(newStudentAddingRequest);
            }

            int byECId = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            foreach (var comment in newRequest.Comments)
            {
                var newComment = new Comment();
                newComment.StaffId = byECId;
                request.Comments.Add(newComment);
            }

            request.ByECId = byECId;
            request.PaymentType = newRequest.PaymentType;
            request.RegistrationStatus = RegistrationStatus.PendingEA;
            request.Type = nameof(RegistrationRequestType.StudentAdding); //TODO Change request.type to enum if need
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();
            return response;
        }
    }
}