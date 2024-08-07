using Firebase.Auth;
using System.Net;

namespace griffined_api.Services.StaffService
{
    public class StaffService : IStaffService
    {
        private readonly string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private readonly string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFirebaseService _firebaseService;

        public StaffService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService)
        {
            _httpContextAccessor = httpContextAccessor;
            _firebaseService = firebaseService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<StaffResponseDto>> AddStaff(AddStaffRequestDto newStaff)
        {
            var response = new ServiceResponse<StaffResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = "hog" + newStaff.Phone;
            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newStaff.Email, password);
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

            var staff = _mapper.Map<Staff>(newStaff);
            staff.FirebaseId = firebaseId;
            staff.CreatedBy = id;
            staff.LastUpdatedBy = id;
            _context.Staff.Add(staff);

            await _context.SaveChangesAsync();
            await AddStaffFireStoreAsync(staff);

            response.Data = _mapper.Map<StaffResponseDto>(staff);
            return response;
        }

        public async Task<ServiceResponse<List<StaffResponseDto>>> DeleteStaff(int id)
        {
            var response = new ServiceResponse<List<StaffResponseDto>>();

            var dbStaff = await _context.Staff.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException($"Staff with ID '{id}' not found.");

            _context.Staff.Remove(dbStaff);
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _context.Staff.Select(e => _mapper.Map<StaffResponseDto>(e)).ToList();

            return response;
        }

        public async Task<ServiceResponse<StaffResponseDto>> DisableStaff(int id)
        {
            var response = new ServiceResponse<StaffResponseDto>();

            var staff = await _context.Staff.FirstOrDefaultAsync(o => o.Id == id) ?? throw new NotFoundException($"Staff with ID '{id}' not found.");

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
        public async Task<ServiceResponse<StaffResponseDto>> EnableStaff(int id)
        {
            var response = new ServiceResponse<StaffResponseDto>();
            var staff = await _context.Staff.FirstOrDefaultAsync(o => o.Id == id) ?? throw new NotFoundException($"Staff with ID '{id}' not found.");

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

        public async Task<ServiceResponse<List<StaffResponseDto>>> GetStaff()
        {
            var dbStaff = await _context.Staff.ToListAsync();

            var data = dbStaff.Select(s =>
            {
                var staffDto = _mapper.Map<StaffResponseDto>(s);
                staffDto.StaffId = s.Id;
                return staffDto;
            }).ToList();


            var response = new ServiceResponse<List<StaffResponseDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
            };

            return response;
        }

        public async Task<ServiceResponse<StaffResponseDto>> GetStaffById(int id)
        {
            var response = new ServiceResponse<StaffResponseDto>();

            var dbStaff = await _context.Staff.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException($"Staff with ID '{id}' not found.");

            var staff = _mapper.Map<StaffResponseDto>(dbStaff);
            staff.StaffId = dbStaff.Id;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = staff;

            return response;
        }

        public async Task<ServiceResponse<StaffResponseDto>> UpdateStaff(UpdateStaffRequestDto updatedStaff)
        {
            var response = new ServiceResponse<StaffResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            var staff = await _context.Staff.FirstOrDefaultAsync(o => o.Id == updatedStaff.Id) ?? throw new NotFoundException($"Staff with ID '{updatedStaff.Id}' not found.");

            _mapper.Map(updatedStaff, staff);

            staff.FirstName = updatedStaff.FirstName;
            staff.LastName = updatedStaff.LastName;
            staff.Nickname = updatedStaff.Nickname;
            staff.Phone = updatedStaff.Phone;
            staff.Line = updatedStaff.Line;
            staff.Email = updatedStaff.Email;
            staff.LastUpdatedBy = id;

            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = staff.FirebaseId,
                Email = updatedStaff.Email
            });

            await AddStaffFireStoreAsync(staff);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StaffResponseDto>(staff);

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

        private async Task AddStaffFireStoreAsync(Staff staff)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(staff.FirebaseId);
            Dictionary<string, object> staffDoc = new()
                {
                    { "displayName", staff.FullName },
                    { "email", staff.Email },
                    { "id", staff.Id },
                    { "role", staff.Role },
                    { "uid", staff.FirebaseId!}
                };
            await docRef.SetAsync(staffDoc);
        }
    }
}