using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        public async Task<ServiceResponse<GetTeacherDto>> AddTeacher(AddTeacherDto newTeacher)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = "hog" + newTeacher.Phone;
            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newTeacher.Email, password);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    throw new ConflictException("Email Exists");
                else if (ex.Message.Contains("INVALID_EMAIL"))
                    throw new ConflictException("Invalid Email Format");
                else
                    throw new InternalServerException("Something went wrong.");
            }

            var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
            string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

            var teacher = _mapper.Map<Teacher>(newTeacher);
            teacher.FirebaseId = firebaseId;
            teacher.CreatedBy = id;
            teacher.LastUpdatedBy = id;
            await AddStaffFireStoreAsync(teacher);
            _context.Teachers.Add(teacher);

            await _context.SaveChangesAsync();
            await AddStaffFireStoreAsync(teacher);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<GetTeacherDto>(teacher);

            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherDto>>> DeleteTeacher(int id)
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();

            var dbTeacher = await _context.Teachers
                .Include(t => t.WorkTimes)
                .FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException($"Teacher with ID {id} not found.");

            _context.Teachers.Remove(dbTeacher);
            _context.WorkTimes.RemoveRange(dbTeacher.WorkTimes);

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _context.Teachers.Select(t => _mapper.Map<GetTeacherDto>(t)).ToList();

            return response;
        }
        public async Task<ServiceResponse<List<GetTeacherDto>>> GetTeacher()
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();

            var dbTeachers = await _context.Teachers
                .Include(t => t.WorkTimes)
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
                .Include(t => t.WorkTimes)
                .FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException($"Teacher with ID {id} not found.");

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
                .Include(t => t.WorkTimes)
                .FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException($"Teacher with ID {id} not found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<GetTeacherDto>(dbTeacher);

            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> UpdateTeacher(UpdateTeacherDto updatedTeacher)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            var teacher = await _context.Teachers
                .Include(t => t.WorkTimes)
                .FirstOrDefaultAsync(t => t.Id == updatedTeacher.Id) ?? throw new NotFoundException($"Teacher with ID {updatedTeacher.Id} not found.");

            _mapper.Map(updatedTeacher, teacher);

            teacher.FirstName = updatedTeacher.FirstName;
            teacher.LastName = updatedTeacher.LastName;
            teacher.Nickname = updatedTeacher.Nickname;
            teacher.Email = updatedTeacher.Email;
            teacher.Line = updatedTeacher.Line;
            teacher.IsActive = updatedTeacher.IsActive;
            teacher.LastUpdatedBy = id;

            if (updatedTeacher.WorkTimes != null)
            {
                teacher.WorkTimes.Clear();
                foreach (var updatedWorkTime in updatedTeacher.WorkTimes)
                {
                    var workTime = _mapper.Map<WorkTime>(updatedWorkTime);
                    teacher.WorkTimes.Add(workTime);
                }
            }

            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = teacher.FirebaseId,
                Email = updatedTeacher.Email
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
                    { "uid", staff.FirebaseId}

                };
            await docRef.SetAsync(staffDoc);
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

    }
}
