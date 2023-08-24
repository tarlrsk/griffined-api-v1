using griffined_api.Dtos.StudyCourseDtos;
using griffined_api.Dtos.SubjectDtos;
using griffined_api.Extensions.DateTimeExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
                Room = dbClass.Room,
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
                    StudySubjectId = dbStudySubject.Subject.Id,
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
                                    .Include(c => c.Attendances)
                                        .ThenInclude(a => a.Student)
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
                dbClass.Status = ClassStatus.Checked;
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "success";
            return response;
        }
    }
}