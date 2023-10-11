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
                                        .ToListAsync();

            var data = dbStaffNotifications.Select(sn => new StaffNotificationResponseDto
            {
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
                                        .ToListAsync();

            var data = dbStudentNotifications.Select(sn => new StudentNotificationResponseDto
            {
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
                                        .ToListAsync();

            var data = dbTeacherNotifications.Select(tn => new TeacherNotificationResponseDto
            {
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
                                                .Where(sn => sn.Student.Id == studentId && sn.HasRead == false)
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
                                                .Where(tn => tn.Teacher.Id == teacherId && tn.HasRead == false)
                                                .ToListAsync();

                    foreach (var dbTeacherNotification in dbTeacherNotifications)
                    {
                        dbTeacherNotification.HasRead = true;
                    }

                    await _context.SaveChangesAsync();
                    break;

                case "ec":
                    int ecId = _firebaseService.GetAzureIdWithToken();

                    var dbECNotifications = await _context.StaffNotifications
                                            .Where(en => en.Staff.Id == ecId && en.HasRead == false)
                                            .ToListAsync();

                    foreach (var dbECNotification in dbECNotifications)
                    {
                        dbECNotification.HasRead = true;
                    }

                    await _context.SaveChangesAsync();
                    break;

                case "ea":
                    int eaId = _firebaseService.GetAzureIdWithToken();

                    var dbEANotifications = await _context.StaffNotifications
                                            .Where(en => en.Staff.Id == eaId && en.HasRead == false)
                                            .ToListAsync();

                    foreach (var dbEANotification in dbEANotifications)
                    {
                        dbEANotification.HasRead = true;
                    }

                    await _context.SaveChangesAsync();
                    break;

                case "oa":
                    int oaId = _firebaseService.GetAzureIdWithToken();

                    var dbOANotifications = await _context.StaffNotifications
                                            .Where(on => on.Staff.Id == oaId && on.HasRead == false)
                                            .ToListAsync();

                    foreach (var dbECNotification in dbOANotifications)
                    {
                        dbECNotification.HasRead = true;
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
                                                .FirstOrDefaultAsync(sn => sn.Id == notificationId && sn.HasRead == false)
                                                ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbStudentNotification.HasRead = true;
                    await _context.SaveChangesAsync();
                    break;

                case "teacher":
                    var dbTeacherNotification = await _context.TeacherNotifications
                            .FirstOrDefaultAsync(tn => tn.Id == notificationId && tn.HasRead == false)
                            ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbTeacherNotification.HasRead = true;
                    await _context.SaveChangesAsync();
                    break;

                case "ec":
                    var dbECNotification = await _context.StaffNotifications
                                .FirstOrDefaultAsync(en => en.Id == notificationId && en.HasRead == false)
                                ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbECNotification.HasRead = true;
                    await _context.SaveChangesAsync();
                    break;

                case "ea":
                    var dbEANotification = await _context.StaffNotifications
                                .FirstOrDefaultAsync(en => en.Id == notificationId && en.HasRead == false)
                                ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbEANotification.HasRead = true;
                    await _context.SaveChangesAsync();
                    break;

                case "oa":
                    var dbOANotification = await _context.StaffNotifications
                                .FirstOrDefaultAsync(on => on.Id == notificationId && on.HasRead == false)
                                ?? throw new NotFoundException($"Notification with ID {notificationId} not found.");

                    dbOANotification.HasRead = true;
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