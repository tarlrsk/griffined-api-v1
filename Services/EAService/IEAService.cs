using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.EAService
{
    public interface IEAService
    {
        Task<ServiceResponse<List<GetEADto>>> GetEA();
        Task<ServiceResponse<GetEADto>> GetEAById(int id);
        Task<ServiceResponse<GetEADto>> AddEA(AddEADto newEA);
        Task<ServiceResponse<GetEADto>> UpdateEA(UpdateEADto updatedEA);
        Task<ServiceResponse<List<GetEADto>>> DeleteEA(int id);
    }
}