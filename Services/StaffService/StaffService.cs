using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace griffined_api.Services.StaffService
{
    public class StaffService : IStaffService
    {
        private string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StaffService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;

        }

        public async Task<ServiceResponse<StaffResponseDto>> AddStaff(AddStaffRequestDto newStaff)
        {
            var response = new ServiceResponse<StaffResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = "hog" + newStaff.phone;
            FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;
            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newStaff.email, password);
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
            staff.firebaseId = firebaseId;
            staff.CreatedBy = id;
            staff.LastUpdatedBy = id;
            _context.Staff.Add(staff);

            await _context.SaveChangesAsync();
            await addStaffFireStoreAsync(staff);

            response.Data = _mapper.Map<StaffResponseDto>(staff);
            return response;
        }

        public async Task<ServiceResponse<List<StaffResponseDto>>> DeleteStaff(int id)
        {
            var response = new ServiceResponse<List<StaffResponseDto>>();

            var dbStaff = await _context.Staff.FirstOrDefaultAsync(e => e.id == id);
            if (dbStaff is null)
                throw new NotFoundException($"Staff with ID '{id}' not found.");

            _context.Staff.Remove(dbStaff);
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _context.Staff.Select(e => _mapper.Map<StaffResponseDto>(e)).ToList();
            response.Message = $"Successfully deleted staff with ID {id}";

            return response;
        }

        public async Task<ServiceResponse<StaffResponseDto>> DisableStaff(int id)
        {
            var response = new ServiceResponse<StaffResponseDto>();

            var staff = await _context.Staff.FirstOrDefaultAsync(o => o.id == id);

            if (staff is null)
                throw new NotFoundException($"Staff with ID '{id}' not found.");

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = staff.firebaseId,
                Disabled = true
            });

            staff.isActive = false;

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Message = $"Sucessfully disabled staff with ID {id}";

            return response;
        }
        public async Task<ServiceResponse<StaffResponseDto>> EnableStaff(int id)
        {
            var response = new ServiceResponse<StaffResponseDto>();
            var staff = await _context.Staff.FirstOrDefaultAsync(o => o.id == id);

            if (staff is null)
                throw new NotFoundException($"Staff with ID '{id}' not found.");

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = staff.firebaseId,
                Disabled = false
            });

            staff.isActive = true;
            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Message = $"Sucessfully enabled staff with ID {id}";

            return response;
        }

        public async Task<ServiceResponse<List<StaffResponseDto>>> GetStaff()
        {
            var response = new ServiceResponse<List<StaffResponseDto>>();
            var dbStaff = await _context.Staff.ToListAsync();

            if (dbStaff is null)
                throw new NotFoundException("No staff found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = dbStaff.Select(e => _mapper.Map<StaffResponseDto>(e)).ToList();
            response.Message = "Sucessfully get all staff.";

            return response;
        }

        public async Task<ServiceResponse<StaffResponseDto>> GetStaffById(int id)
        {
            var response = new ServiceResponse<StaffResponseDto>();

            var dbStaff = await _context.Staff.FirstOrDefaultAsync(e => e.id == id);

            if (dbStaff is null)
                throw new NotFoundException($"Staff with ID '{id}' not found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StaffResponseDto>(dbStaff);
            response.Message = $"Sucessfully get staff with ID {id}";

            return response;
        }

        public async Task<ServiceResponse<StaffResponseDto>> UpdateStaff(UpdateStaffRequestDto updatedStaff)
        {
            var response = new ServiceResponse<StaffResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            var staff = await _context.Staff.FirstOrDefaultAsync(o => o.id == updatedStaff.id);
            if (staff is null)
                throw new NotFoundException($"Staff with ID '{updatedStaff.id}' not found.");

            _mapper.Map(updatedStaff, staff);

            staff.fName = updatedStaff.fName;
            staff.lName = updatedStaff.lName;
            staff.nickname = updatedStaff.nickname;
            staff.phone = updatedStaff.phone;
            staff.line = updatedStaff.line;
            staff.email = updatedStaff.email;
            staff.LastUpdatedBy = id;

            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = staff.firebaseId,
                Email = updatedStaff.email
            });

            await addStaffFireStoreAsync(staff);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StaffResponseDto>(staff);
            response.Message = $"Sucessfully updated staff with ID {id}";

            return response;
        }



        private async Task addStaffFireStoreAsync(Staff staff)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(staff.firebaseId);
            Dictionary<string, object> staffDoc = new Dictionary<string, object>()
                {
                    { "displayName", staff.fullName },
                    { "email", staff.email },
                    { "id", staff.id },
                    { "role", staff.role },
                    { "uid", staff.firebaseId}

                };
            await docRef.SetAsync(staffDoc);
        }

    }
}