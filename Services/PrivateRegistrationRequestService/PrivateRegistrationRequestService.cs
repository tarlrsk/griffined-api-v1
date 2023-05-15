using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.PrivateRegistrationRequestService
{
    public class PrivateRegistrationRequestService : IPrivateRegistrationRequestService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public PrivateRegistrationRequestService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>> AddPrivateRegistrationRequest(AddPrivateRegReqWithInfoDto newRequest)
        {
            var response = new ServiceResponse<List<GetPrivateRegReqWithInfoDto>>();

            try
            {
                var request = _mapper.Map<PrivateRegistrationRequest>(newRequest.request);
                var information = _mapper.Map<List<PrivateRegistrationRequestInfo>>(newRequest.information);
                var preferredDays = _mapper.Map<List<PreferredDay>>(newRequest.information.SelectMany(i => i.preferredDays));

                if (newRequest.request.takenByEPId == 0)
                    request.takenByEPId = null;

                var ep = await _context.Staffs
                    .Include(e => e.privateRegistrationRequests)
                    .FirstOrDefaultAsync(e => e.id == newRequest.request.takenByEPId);

                if (ep is null)
                    throw new Exception($"EP with ID {newRequest.request.takenByEPId} not found.");

                request.takenByEPId = newRequest.request.takenByEPId;


                var ea = await _context.Staffs
                    .Include(e => e.privateRegistrationRequests)
                    .FirstOrDefaultAsync(e => e.id == newRequest.request.takenByEAId);

                if (ea is not null)
                {
                    request.takenByEAId = newRequest.request.takenByEAId;
                }


                var oa = await _context.Staffs
                    .Include(o => o.privateRegistrationRequests)
                    .FirstOrDefaultAsync(o => o.id == newRequest.request.takenByOAId);

                if (oa is not null)
                {
                    request.takenByOAId = newRequest.request.takenByOAId;
                }

                var studentIds = newRequest.studentIds ?? new List<int>();
                var students = await _context.Students
                    .Include(s => s.privateRegistrationRequests)
                    .Where(s => studentIds.Contains(s.id))
                    .ToListAsync();

                if (students.Count == 0)
                    throw new Exception("No students found.");

                if (newRequest.studentIds != null)
                    foreach (var studentId in newRequest.studentIds)
                    {
                        var student = await _context.Students.Include(s => s.privateRegistrationRequests)
                            .FirstOrDefaultAsync(s => s.id == studentId);
                        if (student is null)
                            throw new Exception($"Student with id {studentId} not found.");

                        if (student.privateRegistrationRequests != null)
                            student.privateRegistrationRequests.Add(request);

                        request.privateRegistrationRequestInfos.AddRange(information);
                    }

                await _context.SaveChangesAsync();

                response.Data = _context.PrivateRegistrationRequests
                    .Include(r => r.students)
                    .ToList()
                    .Select(r =>
                    {
                        var dto = _mapper.Map<GetPrivateRegReqWithInfoDto>(r);
                        dto.students = r.students.Select(s => new GetStudentNameDto
                        {
                            id = s.id,
                            studentId = s.dateCreated.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (s.id % 10000).ToString("0000"),
                            fullName = s.fullName,
                            nickname = s.nickname
                        }).ToList();
                        return dto;
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>> DeletePrivateRegistrationRequest(int reqId)
        {
            var response = new ServiceResponse<List<GetPrivateRegReqWithInfoDto>>();
            try
            {
                var request = await _context.PrivateRegistrationRequests
                    .Include(r => r.privateRegistrationRequestInfos)
                    .ThenInclude(i => i.preferredDays)
                    .FirstOrDefaultAsync(r => r.id == reqId);

                if (request is null)
                    throw new Exception($"Request with id {reqId} not found.");

                foreach (var student in request.students)
                {
                    student.privateRegistrationRequests?.Remove(request);
                }

                _context.PrivateRegistrationRequests.Remove(request);
                _context.PrivateRegistrationRequestInfos.RemoveRange(request.privateRegistrationRequestInfos);
                _context.PreferredDays.RemoveRange(request.privateRegistrationRequestInfos.SelectMany(i => i.preferredDays));


                await _context.SaveChangesAsync();

                response.Data = _context.PrivateRegistrationRequests
                    .Include(r => r.students)
                    .ToList()
                    .Select(r =>
                    {
                        var dto = _mapper.Map<GetPrivateRegReqWithInfoDto>(r);
                        dto.students = r.students.Select(s => new GetStudentNameDto
                        {
                            id = s.id,
                            studentId = s.dateCreated.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (s.id % 10000).ToString("0000"),
                            fullName = s.fullName,
                            nickname = s.nickname
                        }).ToList();
                        return dto;
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>> GetPrivateRegistrationRequest()
        {
            var response = new ServiceResponse<List<GetPrivateRegReqWithInfoDto>>();
            var requests = await _context.PrivateRegistrationRequests
                .Include(r => r.students)
                .Include(r => r.privateRegistrationRequestInfos)
                .ThenInclude(i => i.preferredDays)
                .ToListAsync();

            foreach (var request in requests)
            {
                if (request.students is null)
                    throw new Exception($"Student is missing.");
            }
            response.Data = requests.Select(r =>
            {
                var dto = _mapper.Map<GetPrivateRegReqWithInfoDto>(r);
                dto.students = r.students.Select(s => new GetStudentNameDto
                {
                    id = s.id,
                    studentId = s.dateCreated.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (s.id % 10000).ToString("0000"),
                    fullName = s.fullName,
                    nickname = s.nickname
                }).ToList();
                return dto;
            })
            .ToList();
            return response;
        }

        public async Task<ServiceResponse<GetPrivateRegReqWithInfoDto>> GetPrivateRegistrationRequestById(int reqId)
        {
            var response = new ServiceResponse<GetPrivateRegReqWithInfoDto>();
            try
            {
                var dbRequests = await _context.PrivateRegistrationRequests
                    .Include(r => r.students)
                    .Include(r => r.privateRegistrationRequestInfos)
                    .ThenInclude(i => i.preferredDays)
                    .FirstOrDefaultAsync(r => r.id == reqId);

                if (dbRequests is null)
                    throw new Exception($"Request with ID '{reqId}' not found.");

                var responseData = _mapper.Map<GetPrivateRegReqWithInfoDto>(dbRequests);
                responseData.students = dbRequests.students.Select(s => new GetStudentNameDto
                {
                    id = s.id,
                    studentId = s.dateCreated.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (s.id % 10000).ToString("0000"),
                    fullName = s.fullName,
                    nickname = s.nickname
                }).ToList();

                response.Data = responseData;
            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetPrivateRegReqWithInfoDto>> UpdatePrivateRegistrationRequest(UpdatePrivateRegReqWithInfoDto updatedRequest)
        {
            var response = new ServiceResponse<GetPrivateRegReqWithInfoDto>();
            try
            {
                var request = await _context.PrivateRegistrationRequests
                    .Include(r => r.students)
                    .FirstOrDefaultAsync(r => r.id == updatedRequest.request.id);

                if (request is null)
                    throw new Exception($"Request with id {updatedRequest.request.id} not found");

                _mapper.Map(updatedRequest.request, request);

                request.status = updatedRequest.request.status;
                request.EAStatus = updatedRequest.request.EAStatus;
                request.OARemark = updatedRequest.request.OARemark;
                request.paymentStatus = updatedRequest.request.paymentStatus;
                request.EPRemark1 = updatedRequest.request.EPRemark1;
                request.EPRemark2 = updatedRequest.request.EPRemark2;
                request.EARemark = updatedRequest.request.EARemark;
                request.OARemark = updatedRequest.request.OARemark;

                if (request.takenByEPId != null)
                    request.takenByEPId = updatedRequest.request.takenByEPId;

                if (updatedRequest.request.takenByEAId == 0)
                {
                    request.takenByEAId = null;
                    if (request.takenByEAId != null)
                        request.takenByEAId = updatedRequest.request.takenByEAId;
                }

                if (request.takenByOAId != null)
                    request.takenByOAId = updatedRequest.request.takenByOAId;

                var updatedEP = await _context.Staffs
                    .Include(e => e.privateRegistrationRequests)
                    .FirstOrDefaultAsync(e => e.id == updatedRequest.request.takenByEPId);

                if (updatedEP is null)
                    throw new Exception($"EP with ID {updatedRequest.request.takenByEPId} not found.");

                await _context.SaveChangesAsync();

                var responseData = _mapper.Map<GetPrivateRegReqWithInfoDto>(request);
                responseData.students = request.students.Select(s => new GetStudentNameDto
                {
                    id = s.id,
                    studentId = s.dateCreated.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (s.id % 10000).ToString("0000"),
                    fullName = s.fullName,
                    nickname = s.nickname
                }).ToList();

                response.Data = responseData;

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