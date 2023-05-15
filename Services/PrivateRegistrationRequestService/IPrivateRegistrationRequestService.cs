using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.PrivateRegistrationRequestService
{
    public interface IPrivateRegistrationRequestService
    {
        // Request Services
        Task<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>> GetPrivateRegistrationRequest();
        Task<ServiceResponse<GetPrivateRegReqWithInfoDto>> GetPrivateRegistrationRequestById(int reqId);
        Task<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>> AddPrivateRegistrationRequest(AddPrivateRegReqWithInfoDto newRequest);
        Task<ServiceResponse<GetPrivateRegReqWithInfoDto>> UpdatePrivateRegistrationRequest(UpdatePrivateRegReqWithInfoDto updatedRequest);
        Task<ServiceResponse<List<GetPrivateRegReqWithInfoDto>>> DeletePrivateRegistrationRequest(int reqId);
    }
}