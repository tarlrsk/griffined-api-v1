using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using griffined_api.Extensions.DateTimeExtensions;
using System.Xml.Linq;

namespace griffined_api.Services.TeacherService
{
    public class TeacherService : ITeacherService
    {
        private readonly string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private readonly string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFirebaseService _firebaseService;
        public TeacherService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService)
        {
            _httpContextAccessor = httpContextAccessor;
            _firebaseService = firebaseService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<GetTeacherDto>> AddTeacher(AddTeacherDto request)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = int.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = "Hog" + request.Phone;
            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(request.Email, password);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    throw new ConflictException("Email Exists");
                }
                else if (ex.Message.Contains("INVALID_EMAIL"))
                {
                    throw new ConflictException("Invalid Email Format");
                }
                else
                {
                    throw new InternalServerException("Something went wrong.");
                }
            }

            var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
            string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

            var newTeacher = new Teacher
            {
                FirebaseId = firebaseId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Nickname = request.Nickname,
                Phone = request.Phone,
                Email = request.Email,
                Line = request.Line,
                CreatedBy = id,
                LastUpdatedBy = id,
            };

            var mandays = MapMandayDTOToModel(request.Mandays, newTeacher).ToList();
            var workDays = mandays.SelectMany(x => MapWorkTimeDTOTMandayModel(request.Mandays
                                  .SelectMany(y => y.WorkDays), x))
                                  .ToList();

            using (var transaction = _context.Database.BeginTransaction())
            {
                _context.Teachers.Add(newTeacher);

                if (mandays.Any())
                {
                    _context.Mandays.AddRange(mandays);

                    if (workDays.Any())
                    {
                        _context.WorkTimes.AddRange(workDays);
                    }
                }

                transaction.Commit();
            }

            _context.SaveChanges();
            await AddStaffFireStoreAsync(newTeacher);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = MapModelToDTO(newTeacher, mandays, workDays);

            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherDto>>> DeleteTeacher(int id)
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();

            var dbTeacher = await _context.Teachers
                .Include(t => t.Mandays)
                    .ThenInclude(x => x.WorkTimes)
                .FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException($"Teacher with ID {id} not found.");

            _context.Teachers.Remove(dbTeacher);
            _context.Mandays.RemoveRange(dbTeacher.Mandays);
            _context.WorkTimes.RemoveRange(dbTeacher.Mandays.SelectMany(x => x.WorkTimes));

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _context.Teachers.Select(t => _mapper.Map<GetTeacherDto>(t)).ToList();

            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherDto>>> GetTeacher()
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();

            var dbTeachers = await _context.Teachers
                                           .Include(t => t.Mandays)
                                               .ThenInclude(x => x.WorkTimes)
                                           .ToListAsync();

            var data = dbTeachers.Select(s =>
            {
                var teacherDto = _mapper.Map<GetTeacherDto>(s);
                teacherDto.TeacherId = s.Id;
                return teacherDto;
            }).ToList();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;

            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> GetTeacherById(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();

            var dbTeacher = await _context.Teachers
                                          .Include(t => t.Mandays)
                                              .ThenInclude(x => x.WorkTimes)
                                          .FirstOrDefaultAsync(t => t.Id == id)
                                          ?? throw new NotFoundException($"Teacher with ID {id} not found.");

            var data = _mapper.Map<GetTeacherDto>(dbTeacher);
            data.TeacherId = dbTeacher.Id;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;

            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> GetTeacherByToken()
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<GetTeacherDto>();

            var dbTeacher = await _context.Teachers
                                          .Include(t => t.Mandays)
                                              .ThenInclude(x => x.WorkTimes)
                                          .FirstOrDefaultAsync(t => t.Id == id)
                                          ?? throw new NotFoundException($"Teacher with ID {id} not found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<GetTeacherDto>(dbTeacher);

            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> UpdateTeacher(UpdateTeacherDto request)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            var teacher = await _context.Teachers.AsNoTracking()
                                                 .Include(x => x.Mandays)
                                                    .ThenInclude(x => x.WorkTimes)
                                                 .FirstOrDefaultAsync(x => x.Id == request.Id)
                                                 ?? throw new NotFoundException($"Teacher with ID {request.Id} not found.");

            var mandays = (from data in request.Mandays
                           select new Manday
                           {
                               TeacherId = teacher.Id,
                               Year = data.Year,
                               WorkTimes = (from workTime in data.WorkDays
                                            select new WorkTime
                                            {
                                                Quarter = workTime.Quarter,
                                                Day = workTime.Day
                                            })
                                            .ToList()
                           })
                           .ToList();

            using (var transaction = _context.Database.BeginTransaction())
            {
                teacher.FirstName = request.FirstName;
                teacher.LastName = request.LastName;
                teacher.Nickname = request.Nickname;
                teacher.Phone = request.Phone;
                teacher.Email = request.Email;
                teacher.Line = request.Line;

                _context.WorkTimes.RemoveRange(teacher.Mandays.SelectMany(x => x.WorkTimes));
                _context.Mandays.RemoveRange(teacher.Mandays);

                if (mandays!.Any())
                {
                    _context.Mandays.AddRange(mandays!);

                    if (mandays.SelectMany(x => x.WorkTimes).Any())
                    {
                        _context.WorkTimes.AddRange(mandays.SelectMany(x => x.WorkTimes));
                    }
                }

                transaction.Commit();
            }

            _context.SaveChanges();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = teacher.FirebaseId,
                Email = request.Email
            });

            await AddStaffFireStoreAsync(teacher);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<GetTeacherDto>(teacher);

            return response;
        }

        public async Task<ServiceResponse<string>> ChangePasswordWithFirebaseId(string uid, ChangeUserPasswordDto password)
        {
            if (password.Password != password.VerifyPassword)
                throw new BadRequestException("Both Password must be the same");

            await _firebaseService.ChangePasswordWithUid(uid, password.Password);

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;

        }

        public async Task<ServiceResponse<GetTeacherDto>> DisableTeacher(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();

            var staff = await _context.Teachers.FirstOrDefaultAsync(o => o.Id == id) ?? throw new NotFoundException($"Teacher with ID '{id}' not found.");

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = staff.FirebaseId,
                Disabled = true
            });


            staff.IsActive = false;
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> EnableTeacher(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();

            var staff = await _context.Teachers.FirstOrDefaultAsync(o => o.Id == id) ?? throw new NotFoundException($"Teacher with ID '{id}' not found.");

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = staff.FirebaseId,
                Disabled = false
            });


            staff.IsActive = true;
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }

        public List<TeacherShiftResponseDto> GetTeacherWorkTypesWithHours(Teacher teacher, DateTime date, TimeSpan fromTime, TimeSpan toTime)
        {
            List<TeacherShiftResponseDto> teacherShifts = new List<TeacherShiftResponseDto>();

            // GET THE MANDAY FOR THE SPECIFIED DATE
            var manday = teacher.Mandays.FirstOrDefault(m => m.Year == date.Year);

            if (manday is not null)
            {
                // GET THE WORK TIMES FOR THE QUARTER AND DAY OF WEEK
                IEnumerable<WorkTime> workTimes = manday.WorkTimes.Where(w => w.Quarter == GetQuarter(date)
                                                                           && w.Day == date.DayOfWeek);

                foreach (WorkTime workTime in workTimes)
                {
                    // CALCULATE THE END TIME BASED ON THE START TIME
                    TimeSpan endTime = fromTime.Add(TimeSpan.FromHours(7));

                    // ADJUST END TIME BASED ON START TIME
                    if (fromTime.Hours < 10)
                    {
                        endTime = endTime.Add(TimeSpan.FromHours(-1));
                    }
                    else if (fromTime.Hours > 10)
                    {
                        endTime = endTime.Add(TimeSpan.FromHours(1));
                    }

                    TeacherWorkType workType;

                    // CHECK IF THE TEACHER WORKS OUTSIDE THE POLICY TIME
                    if (fromTime < TimeSpan.FromHours(10) || fromTime > TimeSpan.FromHours(11))
                    {
                        workType = TeacherWorkType.OVERTIME;
                    }
                    // CHECK IF THE TEACHER WORKS ON A DIFFERENT DAY
                    else if (workTime.Day != date.DayOfWeek)
                    {
                        workType = TeacherWorkType.SPECIAL;
                    }
                    else
                    {
                        workType = TeacherWorkType.NORMAL;
                    }

                    // ADD THE SHIFT TO THE LIST
                    teacherShifts.Add(new TeacherShiftResponseDto
                    {
                        Hours = (toTime - fromTime).TotalHours,
                        TeacherWorkType = workType
                    });
                }
            }

            return teacherShifts;
        }

        private int GetQuarter(DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        private static GetTeacherDto MapModelToDTO(Teacher model, IEnumerable<Manday> mandays, IEnumerable<WorkTime> workTimes)
        {
            var response = new GetTeacherDto
            {
                TeacherId = model.Id,
                FirebaseId = model.FirebaseId!,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Nickname = model.Nickname,
                Phone = model.Phone,
                Email = model.Email,
                Line = model.Line,
                IsActive = model.IsActive,
                Mandays = (from manday in mandays
                           orderby manday.Year
                           select new MandayResponseDto
                           {
                               Year = manday.Year,
                               WorkDays = (from workDay in workTimes
                                           orderby workDay.Quarter
                                           select new WorkTimeResponseDto
                                           {
                                               Day = workDay.Day,
                                               Quarter = workDay.Quarter,
                                           })
                                           .ToList()
                           })
                           .ToList()
            };

            return response;
        }

        private static IEnumerable<Manday> MapMandayDTOToModel(
                       IEnumerable<MandayRequestDto> mandays,
                       Teacher model)
        {
            if (mandays is null)
            {
                return Enumerable.Empty<Manday>();
            }

            var response = (from manday in mandays
                            select new Manday
                            {
                                Teacher = model,
                                Year = manday.Year,
                            })
                            .ToList();

            return response;
        }

        private static IEnumerable<WorkTime> MapWorkTimeDTOTMandayModel(
                       IEnumerable<WorkTimeRequestDto> workTimes,
                       Manday model
        )
        {
            if (workTimes is null)
            {
                return Enumerable.Empty<WorkTime>();
            }

            var response = (from workTime in workTimes
                            select new WorkTime
                            {
                                Manday = model,
                                Day = workTime.Day,
                                Quarter = workTime.Quarter
                            })
                            .ToList();

            return response;
        }

        private async Task AddStaffFireStoreAsync(Teacher staff)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(staff.FirebaseId);
            Dictionary<string, object> staffDoc = new()
                {
                    { "displayName", staff.FullName },
                    { "email", staff.Email },
                    { "id", staff.Id },
                    { "role", "teacher" },
                    { "uid", staff.FirebaseId!}

                };
            await docRef.SetAsync(staffDoc);
        }
    }
}
