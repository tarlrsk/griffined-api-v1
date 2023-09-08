using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using griffined_api.Dtos.NotificationDto;
using griffined_api.Extensions.DateTimeExtensions;

namespace griffined_api.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;
        public NotificationService(DataContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }

        public async Task<ServiceResponse<List<StaffNotificationResponseDto>>> GetStaffNotifications()
        {
            var response = new ServiceResponse<List<StaffNotificationResponseDto>>();

            int staffId = _firebaseService.GetAzureIdWithToken();

            var dbStaffNotifications = await _context.StaffNotifications
                                        .Where(sn => sn.Staff.Id == staffId)
                                        .ToListAsync();

            var data = dbStaffNotifications.Select(sn => new StaffNotificationResponseDto
            {
                StaffId = staffId,
                StudyCourseId = sn.StudyCourseId,
                RegistrationRequestId = sn.RegistrationRequestId,
                CancellationRequestId = sn.CancellationRequestId,
                Title = sn.Title,
                Message = sn.Message,
                DateCreated = sn.DateCreated.ToDateTimeString(),
                Type = sn.Type,
                HasRead = sn.HasRead
            }).ToList();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<List<StudentNotificationResponseDto>>> GetStudentNotifications()
        {
            var response = new ServiceResponse<List<StudentNotificationResponseDto>>();

            int studentId = _firebaseService.GetAzureIdWithToken();

            var dbStudentNotifications = await _context.StudentNotifications
                                        .Where(sn => sn.Student.Id == studentId)
                                        .ToListAsync();

            var data = dbStudentNotifications.Select(sn => new StudentNotificationResponseDto
            {
                StudentId = studentId,
                StudyCourseId = sn.StudyCourseId,
                Title = sn.Title,
                Message = sn.Message,
                DateCreated = sn.DateCreated.ToDateTimeString(),
                Type = sn.Type,
                HasRead = sn.HasRead
            }).ToList();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }

        public async Task<ServiceResponse<List<TeacherNotificationResponseDto>>> GetTeacherNotifications()
        {
            var response = new ServiceResponse<List<TeacherNotificationResponseDto>>();

            int teacherId = _firebaseService.GetAzureIdWithToken();

            var dbTeacherNotifications = await _context.TeacherNotifications
                                        .Where(tn => tn.Teacher.Id == teacherId)
                                        .ToListAsync();

            var data = dbTeacherNotifications.Select(tn => new TeacherNotificationResponseDto
            {
                TeacherId = teacherId,
                StudyCourseId = tn.StudyCourseId,
                Title = tn.Title,
                Message = tn.Message,
                DateCreated = tn.DateCreated.ToDateTimeString(),
                Type = tn.Type,
                HasRead = tn.HasRead
            }).ToList();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;
            return response;
        }
    }
}