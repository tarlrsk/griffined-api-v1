using Google.Api;
using griffined_api.Dtos.AppointentDtos;
using griffined_api.Dtos.ScheduleDtos;
using griffined_api.Extensions.DateTimeExtensions;
using System.Net;

namespace griffined_api.Services.AppointmentService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;
        private readonly IAsyncRepository<Appointment> _appointmentRepo;
        private readonly IAsyncRepository<AppointmentSlot> _appointmentSlotRepo;
        private readonly IAsyncRepository<AppointmentMember> _appointmentMemberRepo;
        private readonly IAsyncRepository<Schedule> _scheduleRepo;
        private readonly IAsyncRepository<Staff> _staffRepo;
        private readonly IAsyncRepository<Teacher> _teacherRepo;
        private readonly IAsyncRepository<TeacherNotification> _teacherNotificationRepo;
        private readonly IScheduleService _scheduleService;

        public AppointmentService(IUnitOfWork uow,
                                  DataContext context,
                                  IFirebaseService firebaseService,
                                  IAsyncRepository<Appointment> appointmentRepo,
                                  IAsyncRepository<AppointmentSlot> appointmentSlotRepo,
                                  IAsyncRepository<AppointmentMember> appointmentMemberRepo,
                                  IAsyncRepository<Schedule> scheduleRepo,
                                  IAsyncRepository<Staff> staffRepo,
                                  IAsyncRepository<Teacher> teacherRepo,
                                  IAsyncRepository<TeacherNotification> teacherNotificationRepo,
                                  IScheduleService scheduleService)
        {
            _uow = uow;
            _context = context;
            _firebaseService = firebaseService;
            _appointmentRepo = appointmentRepo;
            _appointmentSlotRepo = appointmentSlotRepo;
            _appointmentMemberRepo = appointmentMemberRepo;
            _scheduleRepo = scheduleRepo;
            _staffRepo = staffRepo;
            _teacherRepo = teacherRepo;
            _teacherNotificationRepo = teacherNotificationRepo;
            _scheduleService = scheduleService;
        }

        public Appointment CreateAppointment(CreateAppointmentDTO request)
        {
            // CHECK IF THE REQUESTED SCHEDULE IS VALID.
            List<string> dates = new List<string>();
            List<string> days = new List<string>();

            foreach (var schedule in request.Schedules)
            {
                dates.Add(schedule.Date);
                days.Add(schedule.Date.ToGregorianDateTime()
                                 .DayOfWeek.ToString());

                var scheduleDTO = new CheckAvailableAppointmentScheduleDTO
                {
                    TeacherIds = request.TeacherIds,
                    Dates = dates,
                    Days = days,
                    FromTime = schedule.FromTime,
                    ToTime = schedule.ToTime,
                    AppointmentType = request.AppointmentType,
                };

                var verifySchedule = _scheduleService.GenerateAvailableAppointmentSchedule(scheduleDTO);

                if (verifySchedule.Data is null)
                {
                    throw new BadHttpRequestException("No schedules.");
                }
            }

            var staffId = _firebaseService.GetAzureIdWithToken();

            var staff = _staffRepo.Query()
                                  .FirstOrDefault(x => x.Id == staffId);

            if (staff is null)
            {
                throw new NotFoundException($"Staff with id ({staffId}) is not found.");
            }

            var appointment = new Appointment
            {
                Title = request.Title,
                Description = request.Description,
                AppointmentType = request.AppointmentType,
                CreatedByStaffId = staff.Id
            };

            _uow.BeginTran();
            _appointmentRepo.Add(appointment);
            _uow.Complete();
            _uow.CommitTran();

            return appointment;
        }

        public void CreateAppointmentMember(IEnumerable<int> teacherIds, Appointment appointment)
        {
            var teachers = _teacherRepo.Query()
                                       .Where(x => teacherIds.Contains(x.Id))
                                       .ToList();

            if (!teachers.Any())
            {
                return;
            }

            var members = (from teacher in teachers
                           select new AppointmentMember
                           {
                               AppointmentId = appointment.Id,
                               TeacherId = teacher.Id,
                           })
                          .ToList();

            _uow.BeginTran();
            _appointmentMemberRepo.AddRange(members);
            _uow.Complete();
            _uow.CommitTran();
        }

        public void CreateAppointmentNotification(IEnumerable<int> teacherIds, Appointment appointment)
        {
            var teachers = _teacherRepo.Query()
                                       .Where(x => teacherIds.Contains(x.Id))
                                       .ToList();

            if (!teachers.Any())
            {
                return;
            }

            var notifications = (from teacher in teachers
                                 select new TeacherNotification
                                 {
                                     TeacherId = teacher.Id,
                                     AppointmentId = appointment.Id,
                                     Title = "New Appointment",
                                     Message = "You have been added to a new Appointment.",
                                     Type = TeacherNotificationType.NewAppointment,
                                     HasRead = false
                                 })
                                .ToList();

            _uow.BeginTran();
            _teacherNotificationRepo.AddRange(notifications);
            _uow.Complete();
            _uow.CommitTran();
        }

        public IEnumerable<Schedule> CreateAppointmentSchedule(CreateAppointmentDTO request, Appointment appointment)
        {
            var schedules = (from schedule in request.Schedules
                             select new Schedule
                             {
                                 Date = schedule.Date.ToGregorianDateTime(),
                                 FromTime = schedule.FromTime,
                                 ToTime = schedule.ToTime,
                                 Type = ScheduleType.Appointment,
                                 CalendarType = request.AppointmentType == AppointmentType.HOLIDAY ? DailyCalendarType.HOLIDAY
                                                                                                   : DailyCalendarType.EVENT
                             })
                            .ToList();

            _uow.BeginTran();
            _scheduleRepo.AddRange(schedules);
            _uow.Complete();
            _uow.CommitTran();

            return schedules;
        }

        public void CreateAppointmentSlot(IEnumerable<Schedule> schedules, Appointment appointment)
        {
            var slots = (from schedule in schedules
                         select new AppointmentSlot
                         {
                             ScheduleId = schedule.Id,
                             AppointmentId = appointment.Id,
                             AppointmentSlotStatus = AppointmentSlotStatus.NONE
                         })
                        .ToList();

            _uow.BeginTran();
            _appointmentSlotRepo.AddRange(slots);
            _uow.Complete();
            _uow.CommitTran();
        }

        public async Task<ServiceResponse<List<AppointmentResponseDto>>> ListAllAppointments()
        {
            var dbAppointments = await _context.Appointments
                                               .Include(a => a.AppointmentSlots.Where(a => a.AppointmentSlotStatus != AppointmentSlotStatus.DELETED))
                                                .ThenInclude(a => a.Schedule)
                                               .Include(a => a.Staff)
                                               .ToListAsync();

            var data = new List<AppointmentResponseDto>();

            foreach (var appointment in dbAppointments)
            {
                var response = new AppointmentResponseDto
                {
                    AppointmentId = appointment.Id,
                    AppointmentType = appointment.AppointmentType,
                    Title = appointment.Title,
                    Description = appointment.Description,
                    StartDate = !appointment.AppointmentSlots.Any() ? null
                                                                     : appointment.AppointmentSlots.Min(a => a.Schedule.Date).ToDateString(),
                    EndDate = !appointment.AppointmentSlots.Any() ? null
                                                                   : appointment.AppointmentSlots.Max(a => a.Schedule.Date).ToDateString(),
                    CreatedBy = new StaffNameOnlyResponseDto
                    {
                        StaffId = appointment.Staff?.Id,
                        FirstName = appointment.Staff?.FirstName,
                        LastName = appointment.Staff?.LastName,
                        FullName = appointment.Staff?.FullName,
                        Nickname = appointment.Staff?.Nickname,
                    },
                    Status = appointment.AppointmentStatus,
                };

                data.Add(response);
            }

            // var data = (from appointment in dbAppointments
            //             select new AppointmentResponseDto
            //             {
            //                 AppointmentId = appointment.Id,
            //                 AppointmentType = appointment.AppointmentType,
            //                 Title = appointment.Title,
            //                 Description = appointment.Description,
            //                 StartDate = appointment.AppointmentSlots.Min(a => a.Schedule.Date).ToDateString(),
            //                 EndDate = appointment.AppointmentSlots.Max(a => a.Schedule.Date).ToDateString(),
            //                 CreatedBy = new StaffNameOnlyResponseDto
            //                 {
            //                     StaffId = appointment.Staff?.Id,
            //                     FirstName = appointment.Staff?.FirstName,
            //                     LastName = appointment.Staff?.LastName,
            //                     FullName = appointment.Staff?.FullName,
            //                     Nickname = appointment.Staff?.Nickname,
            //                 },
            //                 Status = appointment.AppointmentStatus,
            //             })
            //            .ToList();

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
                                    .Where(s => s.AppointmentSlotStatus != AppointmentSlotStatus.DELETED))
                                    .ThenInclude(s => s.Schedule)
                                .Include(a => a.AppointmentHistories)
                                    .ThenInclude(a => a.Teacher)
                                .Include(a => a.AppointmentHistories)
                                    .ThenInclude(a => a.AppointmentSlot)
                                        .ThenInclude(a => a!.Schedule)
                                .Include(a => a.AppointmentHistories)
                                    .ThenInclude(a => a.Staff)
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
                    Day = dbSlot.Schedule.Date.DayOfWeek.ToString().ToUpper(),
                    Date = dbSlot.Schedule.Date.ToDateString(),
                    FromTime = dbSlot.Schedule.FromTime,
                    ToTime = dbSlot.Schedule.ToTime,
                    Hour = (dbSlot.Schedule.ToTime - dbSlot.Schedule.FromTime).TotalHours
                });
            }

            foreach (var dbAppointmentHistory in dbAppointment.AppointmentHistories)
            {
                var description = "";
                switch (dbAppointmentHistory.Method)
                {
                    case AppointmentHistoryMethod.AddMember:
                        description = $"Added Teacher {dbAppointmentHistory.Teacher?.FirstName} {dbAppointmentHistory.Teacher?.LastName[0]}. ({dbAppointmentHistory.Teacher?.Nickname}) to {dbAppointment.Title}";
                        data.History.Add(new AppointmentHistoryResponseDto
                        {
                            RecordType = dbAppointmentHistory.Type,
                            Date = dbAppointmentHistory.UpdatedDate.ToDateTimeString(),
                            Record = $"[{dbAppointmentHistory.Staff?.Role.ToUpper()} {dbAppointmentHistory.Staff?.Nickname}/{dbAppointmentHistory?.Staff?.FirstName}] {description}"
                        });
                        break;
                    case AppointmentHistoryMethod.RemoveMember:
                        description = $"Removed Teacher {dbAppointmentHistory.Teacher?.FirstName} {dbAppointmentHistory.Teacher?.LastName[0]}. ({dbAppointmentHistory.Teacher?.Nickname}) to {dbAppointment.Title}";
                        data.History.Add(new AppointmentHistoryResponseDto
                        {
                            RecordType = dbAppointmentHistory.Type,
                            Date = dbAppointmentHistory.UpdatedDate.ToDateTimeString(),
                            Record = $"[{dbAppointmentHistory.Staff?.Role.ToUpper()} {dbAppointmentHistory.Staff?.Nickname}/{dbAppointmentHistory?.Staff?.FirstName}] {description}"
                        });
                        break;
                    case AppointmentHistoryMethod.AddSchedule:
                        description = $"Added {dbAppointment.Title} on {dbAppointmentHistory.AppointmentSlot?.Schedule.Date.ToDateWithDayString()} ({dbAppointmentHistory.AppointmentSlot?.Schedule.FromTime.ToTimeSpanString()} - {dbAppointmentHistory.AppointmentSlot?.Schedule.ToTime.ToTimeSpanString()}).";
                        data.History.Add(new AppointmentHistoryResponseDto
                        {
                            RecordType = dbAppointmentHistory.Type,
                            Date = dbAppointmentHistory.UpdatedDate.ToDateTimeString(),
                            Record = $"[{dbAppointmentHistory.Staff?.Role.ToUpper()} {dbAppointmentHistory.Staff?.Nickname}/{dbAppointmentHistory?.Staff?.FirstName}] {description}"
                        });
                        break;
                    case AppointmentHistoryMethod.RemoveSchedule:
                        description = $"Removed {dbAppointment.Title} on {dbAppointmentHistory.AppointmentSlot?.Schedule.Date.ToDateWithDayString()} ({dbAppointmentHistory.AppointmentSlot?.Schedule.FromTime.ToTimeSpanString()} - {dbAppointmentHistory.AppointmentSlot?.Schedule.ToTime.ToTimeSpanString()}).";
                        data.History.Add(new AppointmentHistoryResponseDto
                        {
                            RecordType = dbAppointmentHistory.Type,
                            Date = dbAppointmentHistory.UpdatedDate.ToDateTimeString(),
                            Record = $"[{dbAppointmentHistory.Staff?.Role.ToUpper()} {dbAppointmentHistory.Staff?.Nickname}/{dbAppointmentHistory?.Staff?.FirstName}] {description}"
                        });
                        break;
                    default:
                        throw new InternalServerException("Something went wrong on History");
                }
            }

            return new ServiceResponse<AppointmentDetailResponseDto>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };
        }

        public async Task<ServiceResponse<string>> UpdateApoointmentById(int appointmentId, UpdateAppointmentRequestDto updateAppointmentRequestDto)
        {
            var dbStaff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == _firebaseService.GetAzureIdWithToken())
                        ?? throw new NotFoundException($"Current User Notfound");

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
                                    && a.AppointmentSlotStatus != AppointmentSlotStatus.DELETED)
                                    ?? throw new NotFoundException($"Schedule ID {deleteScheduleId} is not found.");
                deleteSchedule.AppointmentSlotStatus = AppointmentSlotStatus.DELETED;
                deleteSchedule.Schedule.CalendarType = DailyCalendarType.DELETED;

                dbAppointment.AppointmentHistories ??= new List<AppointmentHistory>();
                dbAppointment.AppointmentHistories.Add(new AppointmentHistory
                {
                    Method = AppointmentHistoryMethod.RemoveSchedule,
                    Type = AppointmentHistoryType.Schedule,
                    AppointmentSlot = deleteSchedule,
                    Staff = dbStaff,
                    UpdatedDate = DateTime.Now,
                    Appointment = dbAppointment,
                });
            }

            var dailyCalendarType = DailyCalendarType.EVENT;
            if (dbAppointment.AppointmentType == AppointmentType.HOLIDAY)
            {
                dailyCalendarType = DailyCalendarType.HOLIDAY;
            }

            foreach (var addSchedule in updateAppointmentRequestDto.ScheduleToAdd)
            {
                var appointmentSlot = new AppointmentSlot
                {
                    AppointmentSlotStatus = AppointmentSlotStatus.NONE,
                    Schedule = new Schedule
                    {
                        Date = addSchedule.Date.ToDateTime(),
                        FromTime = addSchedule.FromTime.ToTimeSpan(),
                        ToTime = addSchedule.ToTime.ToTimeSpan(),
                        Type = ScheduleType.Appointment,
                        CalendarType = dailyCalendarType,
                    },
                };

                dbAppointment.AppointmentSlots ??= new List<AppointmentSlot>();
                dbAppointment.AppointmentSlots.Add(appointmentSlot);

                dbAppointment.AppointmentHistories ??= new List<AppointmentHistory>();
                dbAppointment.AppointmentHistories.Add(new AppointmentHistory
                {
                    Method = AppointmentHistoryMethod.AddSchedule,
                    Type = AppointmentHistoryType.Schedule,
                    AppointmentSlot = appointmentSlot,
                    Staff = dbStaff,
                    UpdatedDate = DateTime.Now,
                    Appointment = dbAppointment,
                });
            }

            var deleteMembers = dbAppointment.AppointmentMembers.Where(m => updateAppointmentRequestDto.TeacherToDelete.Contains(m.Teacher.Id)).ToList();

            foreach (var deleteMember in deleteMembers)
            {
                dbAppointment.AppointmentHistories ??= new List<AppointmentHistory>();
                dbAppointment.AppointmentHistories.Add(new AppointmentHistory
                {
                    Teacher = deleteMember.Teacher,
                    Method = AppointmentHistoryMethod.RemoveMember,
                    Type = AppointmentHistoryType.Member,
                    Staff = dbStaff,
                    UpdatedDate = DateTime.Now,
                    Appointment = dbAppointment,
                });
                _context.AppointmentMembers.Remove(deleteMember);
            }

            foreach (var addTeacher in updateAppointmentRequestDto.TeacherToAdd)
            {
                var dbTeacher = dbTeachers.FirstOrDefault(t => t.Id == addTeacher) ?? throw new NotFoundException($"Teacher with ID {addTeacher} is not found.");
                if (dbAppointment.AppointmentMembers.Any(m => m.Teacher == dbTeacher))
                {
                    throw new BadRequestException($"Teacher ID {dbTeacher.Id} is already exist.");
                }
                else
                {
                    dbAppointment.AppointmentMembers ??= new List<AppointmentMember>();
                    dbAppointment.AppointmentMembers.Add(new AppointmentMember
                    {
                        Teacher = dbTeacher,
                    });
                    dbAppointment.AppointmentHistories ??= new List<AppointmentHistory>();
                    dbAppointment.AppointmentHistories.Add(new AppointmentHistory
                    {
                        Teacher = dbTeacher,
                        Method = AppointmentHistoryMethod.AddMember,
                        Type = AppointmentHistoryType.Member,
                        Staff = dbStaff,
                        UpdatedDate = DateTime.Now,
                        Appointment = dbAppointment,
                    });
                }
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        public void DeleteAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(x => x.Id == id);

            if (appointment is null)
            {
                return;
            }

            _uow.BeginTran();
            _context.Appointments.Remove(appointment);
            _uow.Complete();
            _uow.CommitTran();
        }

        public void DeleteAppointmentMember(int id)
        {
            var appointmentMembers = _context.AppointmentMembers.Where(x => x.AppointmentId == id);

            _uow.BeginTran();

            if (appointmentMembers.Any())
            {
                _context.AppointmentMembers.RemoveRange(appointmentMembers);
            }

            _uow.Complete();
            _uow.CommitTran();
        }

        public void DeleteAppointmentSchedule(int id)
        {
            var appointmentSlots = _context.AppointmentSlots.Where(x => x.AppointmentId == id)
                                                            .ToList();

            var appointmentSlotIds = _context.AppointmentSlots.Where(x => x.AppointmentId == id)
                                                              .Select(x => x.Id)
                                                              .ToList();

            var schedules = _context.Schedules.Include(x => x.AppointmentSlot)
                                              .Where(x => x.AppointmentSlot != null
                                                       && appointmentSlotIds.Contains(x.AppointmentSlot.Id))
                                              .ToList();

            _uow.BeginTran();

            if (appointmentSlots.Any())
            {
                _context.AppointmentSlots.RemoveRange(appointmentSlots);
            }

            if (schedules.Any())
            {
                _context.Schedules.RemoveRange(schedules);
            }

            _uow.Complete();
            _uow.CommitTran();
        }

        public void DeleteTeacherAppointmentNotification(int id)
        {
            var notifications = _context.TeacherNotifications.Where(x => x.AppointmentId != null
                                                                      && x.AppointmentId == id)
                                                             .ToList();

            _uow.BeginTran();

            if (notifications.Any())
            {
                _context.TeacherNotifications.RemoveRange(notifications);
            }

            _uow.Complete();
            _uow.CommitTran();
        }
    }
}