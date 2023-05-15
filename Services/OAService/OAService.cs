using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;

namespace griffined_api.Services.OAService
{
    public class OAService : IOAService
    {

        private const string API_KEY = "AIzaSyDNHYBSm4YOFiKBs6kxdKba_I9klxrazTM";
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public OAService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<GetOADto>> AddOA(AddOADto newOA)
        {
            var response = new ServiceResponse<GetOADto>();

            try
            {
                string password = "hog" + newOA.phone;
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(
                                                    newOA.email, password);
                var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
                string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

                var oa = _mapper.Map<OA>(newOA);
                oa.firebaseId = firebaseId;

                _context.OAs.Add(oa);
                await _context.SaveChangesAsync();
                await addStaffFireStoreAsync(oa);

                response.Data = _mapper.Map<GetOADto>(oa);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetOADto>>> DeleteOA(int id)
        {
            var response = new ServiceResponse<List<GetOADto>>();
            try
            {
                var dbOA = await _context.OAs.FirstOrDefaultAsync(o => o.id == id);
                if (dbOA is null)
                    throw new Exception($"OA with ID '{id}' not found.");

                _context.OAs.Remove(dbOA);
                await _context.SaveChangesAsync();

                response.Data = _context.OAs.Select(o => _mapper.Map<GetOADto>(o)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetOADto>>> GetOA()
        {
            var response = new ServiceResponse<List<GetOADto>>();
            var dbOAs = await _context.OAs.ToListAsync();
            response.Data = dbOAs.Select(o => _mapper.Map<GetOADto>(o)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetOADto>> GetOAById(int id)
        {
            var response = new ServiceResponse<GetOADto>();
            try
            {
                var dbOA = await _context.OAs.FirstOrDefaultAsync(o => o.id == id);
                if (dbOA is null)
                    throw new Exception($"OA with ID '{id}' not found.");
                response.Data = _mapper.Map<GetOADto>(dbOA);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }

        public async Task<ServiceResponse<GetOADto>> UpdateOA(UpdateOADto updatedOA)
        {
            var response = new ServiceResponse<GetOADto>();
            try
            {
                var oa = await _context.OAs.FirstOrDefaultAsync(o => o.id == updatedOA.id);
                if (oa is null)
                    throw new Exception($"OA with ID '{updatedOA.id}' not found.");

                _mapper.Map(updatedOA, oa);

                oa.fName = updatedOA.fName;
                oa.lName = updatedOA.lName;
                oa.nickname = updatedOA.nickname;
                oa.phone = updatedOA.phone;
                oa.line = updatedOA.line;
                oa.email = updatedOA.email;

                await _context.SaveChangesAsync();
                
                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = oa.firebaseId,
                    Email = updatedOA.email
                });

                await addStaffFireStoreAsync(oa);

                response.Data = _mapper.Map<GetOADto>(oa);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        private async Task addStaffFireStoreAsync(OA staff)
        {
            FirestoreDb db = FirestoreDb.Create("myproject-f44ec");
            DocumentReference docRef = db.Collection("users").Document(staff.firebaseId);
            Dictionary<string, object> staffDoc = new Dictionary<string, object>()
                {
                    { "displayName", staff.fullName },
                    { "email", staff.email },
                    { "id", staff.id },
                    { "role", "Office Admin" },
                    { "uid", staff.firebaseId}

                };
            await docRef.SetAsync(staffDoc);
        }
    }
}