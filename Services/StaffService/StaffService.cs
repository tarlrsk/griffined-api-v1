using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;

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

        public async Task<ServiceResponse<GetStaffDto>> AddStaff(AddStaffDto newStaff)
        {
            var response = new ServiceResponse<GetStaffDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            try
            {
                string password = "hog" + newStaff.phone;
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newStaff.email, password);

                var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
                string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

                var staff = _mapper.Map<Staff>(newStaff);
                staff.firebaseId = firebaseId;
                staff.CreatedBy = id;
                staff.LastUpdatedBy = id;
                _context.Staffs.Add(staff);

                await _context.SaveChangesAsync();
                await addStaffFireStoreAsync(staff);

                response.Data = _mapper.Map<GetStaffDto>(staff);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetStaffDto>>> DeleteStaff(int id)
        {
            var response = new ServiceResponse<List<GetStaffDto>>();
            try
            {
                var dbStaff = await _context.Staffs.FirstAsync(e => e.id == id);
                if (dbStaff is null)
                    throw new Exception($"Staff with ID '{id}' not found.");

                _context.Staffs.Remove(dbStaff);
                await _context.SaveChangesAsync();

                response.Data = _context.Staffs.Select(e => _mapper.Map<GetStaffDto>(e)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetStaffDto>> DisableStaff(int id)
        {
            var response = new ServiceResponse<GetStaffDto>();
            try
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(o => o.id == id);
                if (staff is null)
                    throw new Exception($"Staff with ID '{id}' not found.");

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
        public async Task<ServiceResponse<GetStaffDto>> EnableStaff(int id)
        {
            var response = new ServiceResponse<GetStaffDto>();
            try
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(o => o.id == id);
                if (staff is null)
                    throw new Exception($"Staff with ID '{id}' not found.");

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

        public async Task<ServiceResponse<List<GetStaffDto>>> GetStaff()
        {
            var response = new ServiceResponse<List<GetStaffDto>>();
            var dbStaffs = await _context.Staffs.ToListAsync();
            response.Data = dbStaffs.Select(e => _mapper.Map<GetStaffDto>(e)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetStaffDto>> GetStaffById(int id)
        {
            var response = new ServiceResponse<GetStaffDto>();
            try
            {
                var dbStaff = await _context.Staffs.FirstOrDefaultAsync(e => e.id == id);
                if (dbStaff is null)
                    throw new Exception($"Staff with ID '{id}' not found.");
                response.Data = _mapper.Map<GetStaffDto>(dbStaff);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetStaffDto>> UpdateStaff(UpdateStaffDto updatedStaff)
        {
            var response = new ServiceResponse<GetStaffDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            try
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(o => o.id == updatedStaff.id);
                if (staff is null)
                    throw new Exception($"Staff with ID '{updatedStaff.id}' not found.");

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

                response.Data = _mapper.Map<GetStaffDto>(staff);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
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