using griffined_api.Dtos.AppointentDtos;
using griffined_api.Extensions.DateTimeExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace griffined_api.Services.AppointmentService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;

        public AppointmentService(DataContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }

        public async Task<ServiceResponse<string>> AddNewAppointment(NewAppointmentRequestDto newAppointment)
        {
            var dbStaff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == _firebaseService.GetAzureIdWithToken())
                        ?? throw new BadRequestException("Staff is not found.");

            var appointment = new Appointment()
            {
                Title = newAppointment.Title,
                AppointmentType = newAppointment.AppointmentType,
                Description = newAppointment.Description,
                Staff = dbStaff,
            };

            var teachers = await _context.Teachers.ToListAsync();
            foreach (var teacherId in newAppointment.TeacherIds)
            {
                var teacher = teachers.FirstOrDefault(t => t.Id == teacherId) ?? throw new BadRequestException($"Teacher with ID {teacherId} is not found");
                appointment.AppointmentMembers.Add(new AppointmentMember
                {
                    Teacher = teacher,
                });

                var teacherNotification = new TeacherNotification
                {
                    Teacher = teacher,
                    Appointment = appointment,
                    Title = "New Appointment",
                    Message = "You have been added to a new Appointment.",
                    Type = TeacherNotificationType.NewAppointment,
                    HasRead = false
                };

                _context.TeacherNotifications.Add(teacherNotification);
            }

            foreach (var newSchedule in newAppointment.Schedules)
            {
                appointment.AppointmentSlots.Add(new AppointmentSlot
                {
                    Schedule = new Schedule
                    {
                        Date = newSchedule.Date.ToDateTime(),
                        FromTime = newSchedule.FromTime.ToTimeSpan(),
                        ToTime = newSchedule.ToTime.ToTimeSpan(),
                        Type = ScheduleType.Appointment,
                    },
                    AppointmentSlotStatus = AppointmentSlotStatus.None,
                });
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        public async Task<ServiceResponse<List<AppointmentResponseDto>>> ListAllAppointments()
        {
            var dbAppointments = await _context.Appointments
                            .Include(a => a.AppointmentSlots.Where(a => a.AppointmentSlotStatus != AppointmentSlotStatus.Deleted))
                                .ThenInclude(a => a.Schedule)
                            .Include(a => a.Staff)
                            .ToListAsync();

            var data = new List<AppointmentResponseDto>();

            foreach (var dbAppointment in dbAppointments)
            {
                data.Add(new AppointmentResponseDto
                {
                    AppointmentId = dbAppointment.Id,
                    AppointmentType = dbAppointment.AppointmentType,
                    Title = dbAppointment.Title,
                    Description = dbAppointment.Description,
                    StartDate = dbAppointment.AppointmentSlots.Min(a => a.Schedule.Date).ToDateString(),
                    EndDate = dbAppointment.AppointmentSlots.Max(a => a.Schedule.Date).ToDateString(),
                    CreatedBy = new StaffNameOnlyResponseDto
                    {
                        StaffId = dbAppointment.Staff?.Id,
                        FirstName = dbAppointment.Staff?.FirstName,
                        LastName = dbAppointment.Staff?.LastName,
                        FullName = dbAppointment.Staff?.FullName,
                        Nickname = dbAppointment.Staff?.Nickname,
                    },
                    Status = dbAppointment.AppointmentStatus,
                });
            }

            return new ServiceResponse<List<AppointmentResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
        }

        public async Task<ServiceResponse<AppointmentDetailResponseDto>> GetAppointmentById(int appointmentId)
        {
            var dbAppointment = await _context.Appointments
                                .Include(a => a.AppointmentMembers)
                                    .ThenInclude(m => m.Teacher)
                                .Include(a => a.AppointmentSlots
                                    .Where(s => s.AppointmentSlotStatus != AppointmentSlotStatus.Deleted))
                                    .ThenInclude(s => s.Schedule)
                                .FirstOrDefaultAsync(a => a.Id == appointmentId)
                                ?? throw new NotFoundException("Appointment is not found.");

            var data = new AppointmentDetailResponseDto
            {
                AppointmentId = dbAppointment.Id,
                AppointmentType = dbAppointment.AppointmentType,
                Title = dbAppointment.Title,
                Description = dbAppointment.Description,
                Status = dbAppointment.AppointmentStatus,
            };

            foreach (var dbMember in dbAppointment.AppointmentMembers)
            {
                data.Teachers.Add(new TeacherNameResponseDto
                {
                    TeacherId = dbMember.Teacher.Id,
                    FirstName = dbMember.Teacher.FirstName,
                    LastName = dbMember.Teacher.LastName,
                    Nickname = dbMember.Teacher.Nickname,
                    FullName = dbMember.Teacher.FullName,
                });
            }

            foreach (var dbSlot in dbAppointment.AppointmentSlots)
            {
                data.Schedules.Add(new AppointmentScheduleResponseDto
                {
                    ScheduleId = dbSlot.Schedule.Id,
                    Date = dbSlot.Schedule.Date.ToDateString(),
                    FromTime = dbSlot.Schedule.FromTime.ToTimeSpanString(),
                    ToTime = dbSlot.Schedule.ToTime.ToTimeSpanString(),
                });
            }

            return new ServiceResponse<AppointmentDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
        }

        public async Task<ServiceResponse<string>> UpdateApoointmentById(int appointmentId, UpdateAppointmentRequestDto updateAppointmentRequestDto)
        {
            var dbAppointment = await _context.Appointments
                                .Include(a => a.AppointmentSlots)
                                    .ThenInclude(s => s.Schedule)
                                .Include(a => a.AppointmentMembers)
                                    .ThenInclude(m => m.Teacher)
                                .FirstOrDefaultAsync(a => a.Id == appointmentId)
                                ?? throw new NotFoundException($"Appointment With ID {appointmentId} is not found");

            dbAppointment.Title = updateAppointmentRequestDto.Title;
            dbAppointment.AppointmentType = updateAppointmentRequestDto.AppointmentType;
            dbAppointment.Description = updateAppointmentRequestDto.Description;

            var dbTeachers = await _context.Teachers.ToListAsync();


            foreach (var deleteScheduleId in updateAppointmentRequestDto.ScheduleToDelete)
            {
                var deleteSchedule = dbAppointment.AppointmentSlots.FirstOrDefault(a => a.ScheduleId == deleteScheduleId
                                    && a.AppointmentSlotStatus != AppointmentSlotStatus.Deleted)
                                    ?? throw new NotFoundException($"Schedule ID {deleteScheduleId} is not found.");
                deleteSchedule.AppointmentSlotStatus = AppointmentSlotStatus.Deleted;
            }

            foreach (var addSchedule in updateAppointmentRequestDto.ScheduleToAdd)
            {
                dbAppointment.AppointmentSlots.Add(new AppointmentSlot
                {
                    AppointmentSlotStatus = AppointmentSlotStatus.None,
                    Schedule = new Schedule
                    {
                        Date = addSchedule.Date.ToDateTime(),
                        FromTime = addSchedule.FromTime.ToTimeSpan(),
                        ToTime = addSchedule.ToTime.ToTimeSpan(),
                        Type = ScheduleType.Appointment,
                    },
                });
            }

            var deleteMembers = dbAppointment.AppointmentMembers.Where(m => updateAppointmentRequestDto.TeacherToDelete.Contains(m.Teacher.Id)).ToList();

            foreach (var deleteMember in deleteMembers)
            {
                dbAppointment.AppointmentMembers.Remove(deleteMember);
            }

            foreach (var addTeacher in updateAppointmentRequestDto.TeacherToAdd )
            {
                var dbTeacher = dbTeachers.FirstOrDefault(t => t.Id == addTeacher) ?? throw new NotFoundException($"Teacher with ID {addTeacher} is not found.");
                dbAppointment.AppointmentMembers.Add(new AppointmentMember{
                    Teacher = dbTeacher,
                });
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
    }
}