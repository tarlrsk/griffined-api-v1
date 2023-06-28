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

        public RegistrationRequestService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<string>> AddNewRequestedCourses(NewCoursesRequestDto newRequestedCourses)
        {
            var response = new ServiceResponse<string>();
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
                member.studentId = memberId;
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
                        newRequestedSubject.subjectId = subject.id;
                        newRequestedSubject.hour = requestedSubject.hour;
                        newRequestedCourseRequest.NewCourseSubjectRequests.Add(newRequestedSubject);
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
                            newSubject.courseId = course.id;
                            _context.Subjects.Add(newSubject);
                            await _context.SaveChangesAsync();
                            existedCourses = await _context.Courses
                                                .Include(c => c.subjects)
                                                .Where(c => requestedCourses
                                                .Contains(c.course)).ToListAsync();
                            course = existedCourses.First(c => c.course == course.course);

                            newRequestedSubject.subjectId = newSubject.id;
                        }
                        else
                        {
                            newRequestedSubject.subjectId = subject.id;
                        }
                        newRequestedSubject.hour = requestedSubject.hour;
                        newRequestedCourseRequest.NewCourseSubjectRequests.Add(newRequestedSubject);
                    }

                    var level = course.levels.FirstOrDefault(c => c.level == newRequestedCourse.level);
                    if (newRequestedCourse.level != null && level != null)
                    {
                        var newLevel = new Level();
                        newLevel.level = newRequestedCourse.level;
                        newLevel.courseId = course.id;
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

                newRequestedCourseRequest.courseId = course.id;
                newRequestedCourseRequest.totalHours = newRequestedCourse.totalHours;
                newRequestedCourseRequest.method = newRequestedCourseRequest.method;
                newRequestedCourseRequest.startDate = DateTime.Parse(newRequestedCourse.startDate);
                newRequestedCourseRequest.startDate = DateTime.Parse(newRequestedCourse.endDate);
                request.newCourseRequests.Add(newRequestedCourseRequest);
            }
            request.section = newRequestedCourses.sectionName;
            request.registrationStatus = RegistrationStatus.PendingEA;
            _context.RegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            return response;
        }
    }
}