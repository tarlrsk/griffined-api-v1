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
    }
}