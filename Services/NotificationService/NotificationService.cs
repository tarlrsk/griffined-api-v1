using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using griffined_api.Dtos.NotificationDto;
using griffined_api.Extensions.DateTimeExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Diagnostics;

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
                                        .OrderByDescending(sn => sn.DateCreated)
                                        .ToListAsync();

            var data = dbStaffNotifications.Select(sn => new StaffNotificationResponseDto
            {
                Id = sn.Id,
                StaffId = staffId,
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
                                        .OrderByDescending(sn => sn.DateCreated)
                                        .ToListAsync();

            var data = dbStudentNotifications.Select(sn => new StudentNotificationResponseDto
            {
                Id = sn.Id,
                StudentId = studentId,
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
                                        .OrderByDescending(sn => sn.DateCreated)
                                        .ToListAsync();

            var data = dbTeacherNotifications.Select(tn => new TeacherNotificationResponseDto
            {
                Id = tn.Id,
                TeacherId = teacherId,
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

        public async Task<ServiceResponse<string>> MarkAllAsRead()
        {
            string role = _firebaseService.GetRoleWithToken();

            switch (role)
            {
                case "student":
                    int studentId = _firebaseService.GetAzureIdWithToken();

                    var dbStudentNotifications = await _context.StudentNotifications
                                                .Where(sn => sn.Student.Id == studentId)
                                                .ToListAsync();

                    foreach (var dbStudentNotification in dbStudentNotifications)
                    {
                        dbStudentNotification.HasRead = true;
                    }

                    await _context.SaveChangesAsync();
                    break;

                case "teacher":
                    int teacherId = _firebaseService.GetAzureIdWithToken();

                    var dbTeacherNotifications = await _context.TeacherNotifications
                                                .Where(tn => tn.Teacher.Id == teacherId)
                                                .ToListAsync();

                    foreach (var dbTeacherNotification in dbTeacherNotifications)
                    {
                        dbTeacherNotification.HasRead = true;
                    }

                    await _context.SaveChangesAsync();
                    break;

                case "ec":
                case "ea":
                case "oa":
                    int staffId = _firebaseService.GetAzureIdWithToken();

                    var dbStaffNotifications = await _context.StaffNotifications
                                            .Where(sn => sn.Staff.Id == staffId)
                                            .ToListAsync();

                    foreach (var dbStaffNotification in dbStaffNotifications)
                    {
                        dbStaffNotification.HasRead = true;
                    }

                    await _context.SaveChangesAsync();
                    break;
            }

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        public async Task<ServiceResponse<string>> MarkAsRead(int notificationId)
        {
            string role = _firebaseService.GetRoleWithToken();

            switch (role)
            {
                case "student":
                    var dbStudentNotification = await _context.StudentNotifications
                                                .FirstOrDefaultAsync(sn => sn.Id == notificationId)
                                                ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbStudentNotification.HasRead = true;
                    await _context.SaveChangesAsync();
                    break;

                case "teacher":
                    var dbTeacherNotification = await _context.TeacherNotifications
                            .FirstOrDefaultAsync(tn => tn.Id == notificationId)
                            ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbTeacherNotification.HasRead = true;
                    await _context.SaveChangesAsync();
                    break;

                case "ec":
                case "ea":
                case "oa":
                    var dbStaffNotification = await _context.StaffNotifications
                                .FirstOrDefaultAsync(sn => sn.Id == notificationId)
                                ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbStaffNotification.HasRead = true;
                    await _context.SaveChangesAsync();
                    break;
            }

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }
    }
}