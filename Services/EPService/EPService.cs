using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;

namespace griffined_api.Services.EPService
{
    public class EPService : IEPService
    {
        private const string API_KEY = "AIzaSyDNHYBSm4YOFiKBs6kxdKba_I9klxrazTM";
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public EPService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetEPDto>> AddEP(AddEPDto newEP)
        {
            var response = new ServiceResponse<GetEPDto>();
            try
            {
                string password = "hog" + newEP.phone;
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newEP.email, password);

                var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
                string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

                var ep = _mapper.Map<EP>(newEP);
                ep.firebaseId = firebaseId;
                _context.EPs.Add(ep);

                await _context.SaveChangesAsync();
                await addStaffFireStoreAsync(ep);

                response.Data = _mapper.Map<GetEPDto>(ep);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetEPDto>>> DeleteEP(int id)
        {
            var response = new ServiceResponse<List<GetEPDto>>();
            try
            {
                var dbEP = await _context.EPs.FirstAsync(e => e.id == id);
                if (dbEP is null)
                    throw new Exception($"EP with ID '{id}' not found.");

                _context.EPs.Remove(dbEP);
                await _context.SaveChangesAsync();

                response.Data = _context.EPs.Select(e => _mapper.Map<GetEPDto>(e)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetEPDto>>> GetEP()
        {
            var response = new ServiceResponse<List<GetEPDto>>();
            var dbEPs = await _context.EPs.ToListAsync();
            response.Data = dbEPs.Select(e => _mapper.Map<GetEPDto>(e)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetEPDto>> GetEPById(int id)
        {
            var response = new ServiceResponse<GetEPDto>();
            try
            {
                var dbEP = await _context.EPs.FirstOrDefaultAsync(e => e.id == id);
                if (dbEP is null)
                    throw new Exception($"EP with ID '{id}' not found.");
                response.Data = _mapper.Map<GetEPDto>(dbEP);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetEPDto>> UpdateEP(UpdateEPDto updatedEP)
        {
            var response = new ServiceResponse<GetEPDto>();
            try
            {
                var ep = await _context.EPs.FirstOrDefaultAsync(e => e.id == updatedEP.id);
                if (ep is null)
                    throw new Exception($"EP with ID '{updatedEP.id}' not found.");

                _mapper.Map(updatedEP, ep);

                ep.fName = updatedEP.fName;
                ep.lName = updatedEP.lName;
                ep.nickname = updatedEP.nickname;
                ep.phone = updatedEP.phone;
                ep.line = updatedEP.line;
                ep.email = updatedEP.email;
                ep.isActive = updatedEP.isActive;

                await _context.SaveChangesAsync();

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = ep.firebaseId,
                    Email = updatedEP.email
                });

                await addStaffFireStoreAsync(ep);

                response.Data = _mapper.Map<GetEPDto>(ep);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        private async Task addStaffFireStoreAsync(EP staff)
        {
            FirestoreDb db = FirestoreDb.Create("myproject-f44ec");
            DocumentReference docRef = db.Collection("users").Document(staff.firebaseId);
            Dictionary<string, object> staffDoc = new Dictionary<string, object>()
                {
                    { "displayName", staff.fullName },
                    { "email", staff.email },
                    { "id", staff.id },
                    { "role", "Education Planner" },
                    { "uid", staff.firebaseId}

                };
            await docRef.SetAsync(staffDoc);
        }
    }
}