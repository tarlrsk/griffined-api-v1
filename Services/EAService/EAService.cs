using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;

namespace griffined_api.Services.EAService
{
    public class EAService : IEAService
    {
        private const string API_KEY = "AIzaSyDNHYBSm4YOFiKBs6kxdKba_I9klxrazTM";
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public EAService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetEADto>> AddEA(AddEADto newEA)
        {
            var response = new ServiceResponse<GetEADto>();
            try
            {
                string password = "hog" + newEA.phone;
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newEA.email, password);

                var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
                string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;
                var ea = _mapper.Map<EA>(newEA);
                ea.firebaseId = firebaseId;

                _context.EAs.Add(ea);
                await _context.SaveChangesAsync();

                await addStaffFireStoreAsync(ea);

                response.Data = _mapper.Map<GetEADto>(ea);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetEADto>>> DeleteEA(int id)
        {
            var response = new ServiceResponse<List<GetEADto>>();
            try
            {
                var dbEA = await _context.EAs.FirstAsync(e => e.id == id);
                if (dbEA is null)
                    throw new Exception($"EA with ID '{id}' not found.");

                _context.EAs.Remove(dbEA);
                await _context.SaveChangesAsync();

                response.Data = _context.EAs.Select(e => _mapper.Map<GetEADto>(e)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetEADto>>> GetEA()
        {
            var response = new ServiceResponse<List<GetEADto>>();
            var dbEAs = await _context.EAs.ToListAsync();
            response.Data = dbEAs.Select(e => _mapper.Map<GetEADto>(e)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetEADto>> GetEAById(int id)
        {
            var response = new ServiceResponse<GetEADto>();
            try
            {
                var dbEA = await _context.EAs.FirstOrDefaultAsync(e => e.id == id);
                if (dbEA is null)
                    throw new Exception($"EA with ID '{id}' not found.");
                response.Data = _mapper.Map<GetEADto>(dbEA);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetEADto>> UpdateEA(UpdateEADto updatedEA)
        {
            var response = new ServiceResponse<GetEADto>();
            try
            {
                var ea = await _context.EAs.FirstOrDefaultAsync(e => e.id == updatedEA.id);
                if (ea is null)
                    throw new Exception($"EA with ID '{updatedEA.id}' not found.");

                _mapper.Map(updatedEA, ea);

                ea.fName = updatedEA.fName;
                ea.lName = updatedEA.lName;
                ea.nickname = updatedEA.nickname;
                ea.phone = updatedEA.phone;
                ea.line = updatedEA.line;
                ea.email = updatedEA.email;
                ea.isActive = updatedEA.isActive;

                await _context.SaveChangesAsync();

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = ea.firebaseId,
                    Email = updatedEA.email
                });

                await addStaffFireStoreAsync(ea);

                response.Data = _mapper.Map<GetEADto>(ea);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }
        private async Task addStaffFireStoreAsync(EA staff)
        {
            FirestoreDb db = FirestoreDb.Create("myproject-f44ec");
            DocumentReference docRef = db.Collection("users").Document(staff.firebaseId);
            Dictionary<string, object> staffDoc = new Dictionary<string, object>()
                {
                    { "displayName", staff.fullName },
                    { "email", staff.email },
                    { "id", staff.id },
                    { "role", "Education Admin" },
                    { "uid", staff.firebaseId}

                };
            await docRef.SetAsync(staffDoc);
        }
    }
}