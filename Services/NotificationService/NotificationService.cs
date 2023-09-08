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
    }
}