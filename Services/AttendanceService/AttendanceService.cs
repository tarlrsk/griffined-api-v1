using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Extensions.DateTimeExtensions;
using System.Net;

namespace griffined_api.Services.AttendanceService
{
    public class AttendanceService : IAttendanceService
    {
        private readonly DataContext _context;
        public AttendanceService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<AttendanceResponseDto>> GetClassAttendance(int studyClassId)
        {
            var response = new ServiceResponse<AttendanceResponseDto>();

            var dbClass = await _context.StudyClasses
                                .Include(c => c.Schedule)
                                .Include(c => c.StudySubject)
                                    .ThenInclude(ss => ss.StudyCourse)
                                        .ThenInclude(sc => sc.Course)
                                .Include(c => c.StudySubject)
                                    .ThenInclude(ss => ss.Subject)
                                .Include(c => c.Teacher)
                                .Include(c => c.Attendances)
                                    .ThenInclude(a => a.Student)
                                .FirstOrDefaultAsync(c => c.Id == studyClassId) ?? throw new NotFoundException($"No class with ID {studyClassId} found.");

            var dbCourse = await _context.StudyCourses
                                .Include(c => c.StudySubjects)
                                    .ThenInclude(ss => ss.Subject)
                                .FirstOrDefaultAsync(c => c.Id == dbClass.StudySubject.StudyCourse.Id) ?? throw new NotFoundException($"No course found.");

            var data = new AttendanceResponseDto
            {
                StudyClassId = dbClass.Id,
                ClassNo = dbClass.ClassNumber,
                Section = dbClass.StudySubject.StudyCourse.Section,
                Room = dbClass.Schedule.Room,
                Date = dbClass.Schedule.Date.ToDateString(),
                FromTime = dbClass.Schedule.FromTime.ToTimeSpanString(),
                ToTime = dbClass.Schedule.ToTime.ToTimeSpanString(),
                ClassStatus = dbClass.Status,

                CourseId = dbClass.StudySubject.StudyCourse.Id,
                Course = dbClass.StudySubject.StudyCourse.Course.course,
                CourseStatus = dbClass.StudySubject.StudyCourse.Status,
                StudyCourseType = dbClass.StudySubject.StudyCourse.StudyCourseType,

                Subject = dbClass.StudySubject.Subject.subject,

                TeacherId = dbClass.Teacher.Id,
                TeacherFirstName = dbClass.Teacher.FirstName,
                TeacherLastName = dbClass.Teacher.LastName,
                TeacherNickname = dbClass.Teacher.Nickname,

                Members = dbClass.Attendances.Select(student => new StudentAttendanceResponseDto
                {
                    StudentId = student.Student!.Id,
                    StudentFirstName = student.Student.FirstName,
                    StudentLastName = student.Student.LastName,
                    StudentNickname = student.Student.Nickname,
                    Attendance = student.Attendance
                }).ToList()
            };

            foreach (var dbStudySubject in dbCourse.StudySubjects)
            {
                var allSubjectsDto = new StudySubjectResponseDto
                {
                    StudySubjectId = dbStudySubject.Id,
                    SubjectId = dbStudySubject.Subject.Id,
                    Subject = dbStudySubject.Subject.subject
                };

                data.AllSubjects.Add(allSubjectsDto);
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateStudentAttendance(int studyClassId, List<UpdateAttendanceRequestDto> updateAttendanceRequests)
        {
            var response = new ServiceResponse<string>();

            var dbClass = await _context.StudyClasses
                                    .Include(x => x.StudyCourse)
                                    .Include(c => c.Attendances)
                                        .ThenInclude(a => a.Student)
                                    .Include(x => x.Teacher)
                                    .FirstOrDefaultAsync(c => c.Id == studyClassId) ?? throw new NotFoundException($"Class with ID {studyClassId} not found.");

            foreach (var updateAttendanceRequest in updateAttendanceRequests)
            {
                var dbAttendance = dbClass.Attendances
                                    .FirstOrDefault(a => a.StudentId == updateAttendanceRequest.StudentId) ?? throw new NotFoundException($"No student found.");

                dbAttendance.Attendance = updateAttendanceRequest.Attendance;
            }

            bool allAttendancesSet = dbClass.Attendances.All(a => a.Attendance != Attendance.None);

            if (allAttendancesSet)
            {
                dbClass.Status = ClassStatus.CHECKED;
            }

            var dbStudyCourse = dbClass.StudyCourse;

            var dbStudySubjects = await _context.StudySubjects
                    .Include(ss => ss.StudyCourse)
                        .ThenInclude(sc => sc.Course)
                    .Include(ss => ss.Subject)
                    .Include(ss => ss.StudyClasses)
                    .Where(ss => ss.StudyCourse.Id == dbStudyCourse.Id)
                    .ToListAsync()
                    ?? throw new NotFoundException("No Subject Found.");

            int completedClass = 0;
            int incompleteClass = 0;

            foreach (var dbStudySubject in dbStudySubjects)
            {
                foreach (var dbStudyClass in dbStudySubject.StudyClasses)
                {
                    if (dbStudyClass.Status == ClassStatus.CHECKED || dbStudyClass.Status == ClassStatus.UNCHECKED)
                    {
                        completedClass += 1;
                    }
                    else if (dbStudyClass.Status == ClassStatus.NONE)
                    {
                        incompleteClass += 1;
                    }
                }
            }

            double progressRatio = completedClass != 0 ? (double)completedClass / incompleteClass : 0;
            double progress = Math.Round(progressRatio * 100);

            var teacherNotification = new TeacherNotification
            {
                TeacherId = dbClass.Teacher.Id,
                StudyCourseId = dbStudyCourse.Id,
                DateCreated = DateTime.Now,
                Type = TeacherNotificationType.StudentReport,
                HasRead = false
            };

            switch (progress)
            {
                case 50:
                    teacherNotification.Title = "Course Progress Report";
                    teacherNotification.Message = "The course has reached the 50% checkpoint. Please upload the student report. Click for more details.";
                    break;
                case 100:
                    teacherNotification.Title = "Course Progress Report";
                    teacherNotification.Message = "The course has been completed. Please upload the student report. Click for more details.";
                    break;
            }

            if (progress >= 100)
            {
                dbStudyCourse.Status = StudyCourseStatus.Finished;
            }

            _context.TeacherNotifications.Add(teacherNotification);

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "success";
            return response;
        }
    }
}