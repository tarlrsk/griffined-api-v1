using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.EPService
{
    public interface IEPService
    {
        Task<ServiceResponse<List<GetEPDto>>> GetEP();
        Task<ServiceResponse<GetEPDto>> GetEPById(int id);
        Task<ServiceResponse<GetEPDto>> AddEP(AddEPDto newEP);
        Task<ServiceResponse<GetEPDto>> UpdateEP(UpdateEPDto updatedEP);
        Task<ServiceResponse<List<GetEPDto>>> DeleteEP(int id);
    }
}