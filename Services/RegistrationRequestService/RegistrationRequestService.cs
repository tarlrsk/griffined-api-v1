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

        public async Task<ServiceResponse<string>> AddNewCourses(NewCoursesRequestDto newStudyCoursesRequest)
        {
            var response = new ServiceResponse<string>();
            var request = new RegistrationRequest();

            if (newStudyCoursesRequest.memberIds == null || newStudyCoursesRequest.memberIds.Count == 0)
            {
                throw new BadRequestException("The memberIds field is required.");
            }
            foreach (var memberId in newStudyCoursesRequest.memberIds)
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.id == memberId);
                if (dbStudent == null)
                {
                    throw new NotFoundException($"Student with ID {memberId} not found.");
                }
                var member = new RegistrationRequestMember();
                member.studentId = memberId;
                request.registrationRequestMembers.Append(member);
            }

            if (newStudyCoursesRequest.courses == null || newStudyCoursesRequest.courses.Count == 0)
            {
                throw new BadRequestException("The courses field is required.");
            }

            List<string>? coursesRequest = null;
            List<string>? levelRequest = null;
            foreach (var newStudyCourse in newStudyCoursesRequest.courses)
            {
                coursesRequest!.Append(newStudyCourse.course);
                levelRequest!.Append(newStudyCourse.level);
            }
            var coursesResponse = await _context.Courses
                        .Include(c => c.subjects)
                        .Where(c => coursesRequest!.Contains(c.course)).ToListAsync();

            var levelResponse = await _context.Levels
                                .Where(l => levelRequest!.Contains(l.level)).ToListAsync();

            foreach (var newStudyCourse in newStudyCoursesRequest.courses)
            {
                var newStudyCourseRequest = new NewCourseRequest();
                var course = coursesResponse.FirstOrDefault(c => c.course == newStudyCourse.course);
                if (course == null)
                {
                    var newCourse = new Course();
                    newCourse.course = newStudyCourse.course;
                    if (newStudyCourse.subjects == null)
                        throw new BadRequestException("The subjects field is required.");
                    foreach (var newStudySubject in newStudyCourse.subjects)
                    {
                        var newSubject = new Subject();
                        newSubject.subject = newStudySubject.subject;
                        newCourse.subjects.Append(newSubject);
                    }

                    if (newStudyCourse.level != null)
                    {
                        var newLevel = new Level();
                        newLevel.level = newStudyCourse.level;
                        _context.Levels.Append(newLevel); //TODO Change to _context.Course.Level.Append(newLevel) When the Table is ready
                    }

                    _context.Courses.Append(newCourse);
                    await _context.SaveChangesAsync();
                    coursesResponse = await _context.Courses
                        .Include(c => c.subjects)
                        .Where(c => coursesRequest!.Contains(c.course)).ToListAsync();

                    levelResponse = await _context.Levels
                                .Where(l => levelRequest!.Contains(l.level)).ToListAsync();

                    course = coursesResponse.First(c => c.course == newStudyCourse.course);
                    var level = levelResponse.FirstOrDefault(l => l.level == newStudyCourse.level);

                    newStudyCourseRequest.courseId = course.id;
                    newStudyCourseRequest.level = level;
                    newStudyCourseRequest.totalHours = newStudyCourse.totalHours;
                    newStudyCourseRequest.method = newStudyCourseRequest.method;
                    newStudyCourseRequest.startDate = DateTime.Parse(newStudyCourse.startDate);
                    newStudyCourseRequest.startDate = DateTime.Parse(newStudyCourse.endDate);

                    request.newCourseRequests.Append(newStudyCourseRequest);
                }
                else
                {
                    if (newStudyCourse.subjects == null)
                        throw new BadRequestException($"The subjects field is required for {newStudyCourse.course}");
                    foreach (var newStudySubject in newStudyCourse.subjects)
                    {
                        var subject = course.subjects.FirstOrDefault(s => s.subject == newStudySubject.subject);
                        if (subject == null)
                        {
                            var newSubject = new Subject();
                            newSubject.subject = newStudySubject.subject;
                            newSubject.courseId = course.id;
                            _context.Subjects.Append(newSubject);
                            await _context.SaveChangesAsync();
                            coursesResponse = await _context.Courses
                                                .Include(c => c.subjects)
                                                .Where(c => coursesRequest!.Contains(c.course)).ToListAsync();
                        }
                    }
                }
            }

            return response;
        }
    }
}