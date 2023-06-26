using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;

namespace griffined_api.Services.TeacherService
{
    public class TeacherService : ITeacherService
    {
        private string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TeacherService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<GetTeacherDto>> AddTeacher(AddTeacherDto newTeacher)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = "hog" + newTeacher.phone;
            FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;
            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newTeacher.email, password);
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
            teacher.firebaseId = firebaseId;
            teacher.CreatedBy = id;
            teacher.LastUpdatedBy = id;
            await addStaffFireStoreAsync(teacher);
            _context.Teachers.Add(teacher);

            if (newTeacher.workTimes != null)
            {
                teacher.workTimes = new List<WorkTime>();
                foreach (var workTime in newTeacher.workTimes)
                {
                    var day = _mapper.Map<WorkTime>(workTime);
                    teacher.workTimes.Add(day);
                }
            }

            await _context.SaveChangesAsync();
            await addStaffFireStoreAsync(teacher);

            response.Data = _mapper.Map<GetTeacherDto>(teacher);
            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherDto>>> DeleteTeacher(int id)
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();

            try
            {
                var dbTeacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstOrDefaultAsync(t => t.id == id);
                if (dbTeacher is null)
                    throw new NotFoundException($"Teacher with ID {id} not found.");

                _context.Teachers.Remove(dbTeacher);
                _context.WorkTimes.RemoveRange(dbTeacher.workTimes);

                await _context.SaveChangesAsync();

                response.Data = _context.Teachers.Select(t => _mapper.Map<GetTeacherDto>(t)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<List<GetTeacherDto>>> GetTeacher()
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();
            var dbTeachers = await _context.Teachers
                .Include(t => t.workTimes)
                .ToListAsync();
            response.Data = dbTeachers.Select(t => _mapper.Map<GetTeacherDto>(t)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> GetTeacherById(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var dbTeacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstOrDefaultAsync(t => t.id == id);
                if (dbTeacher is null)
                    throw new Exception($"Teacher with ID {id} not found.");
                response.Data = _mapper.Map<GetTeacherDto>(dbTeacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> GetTeacherByToken()
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var dbTeacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstOrDefaultAsync(t => t.id == id);
                if (dbTeacher is null)
                    throw new Exception($"Teacher with ID {id} not found.");
                response.Data = _mapper.Map<GetTeacherDto>(dbTeacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> UpdateTeacher(UpdateTeacherDto updatedTeacher)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstOrDefaultAsync(t => t.id == updatedTeacher.id);
                if (teacher is null)
                    throw new Exception($"Teacher with ID {updatedTeacher.id} not found.");

                _mapper.Map(updatedTeacher, teacher);

                teacher.fName = updatedTeacher.fName;
                teacher.lName = updatedTeacher.lName;
                teacher.nickname = updatedTeacher.nickname;
                teacher.email = updatedTeacher.email;
                teacher.line = updatedTeacher.line;
                teacher.isActive = updatedTeacher.isActive;
                teacher.LastUpdatedBy = id;

                if (updatedTeacher.workTimes != null)
                {
                    teacher.workTimes.Clear();
                    foreach (var updatedWorkTime in updatedTeacher.workTimes)
                    {
                        var workTime = _mapper.Map<WorkTime>(updatedWorkTime);
                        teacher.workTimes.Add(workTime);
                    }
                }

                await _context.SaveChangesAsync();

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = teacher.firebaseId,
                    Email = updatedTeacher.email
                });

                await addStaffFireStoreAsync(teacher);

                response.Data = _mapper.Map<GetTeacherDto>(teacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private async Task addStaffFireStoreAsync(Teacher staff)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(staff.firebaseId);
            Dictionary<string, object> staffDoc = new Dictionary<string, object>()
                {
                    { "displayName", staff.fullName },
                    { "email", staff.email },
                    { "id", staff.id },
                    { "role", "Teacher" },
                    { "uid", staff.firebaseId}

                };
            await docRef.SetAsync(staffDoc);
        }

        public async Task<ServiceResponse<GetTeacherDto>> DisableTeacher(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var staff = await _context.Teachers.FirstOrDefaultAsync(o => o.id == id);
                if (staff is null)
                    throw new Exception($"Teacher with ID '{id}' not found.");

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = staff.firebaseId,
                    Disabled = true
                });


                staff.isActive = false;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> EnableTeacher(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var staff = await _context.Teachers.FirstOrDefaultAsync(o => o.id == id);
                if (staff is null)
                    throw new Exception($"Teacher with ID '{id}' not found.");

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = staff.firebaseId,
                    Disabled = false
                });


                staff.isActive = true;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

    }
}
