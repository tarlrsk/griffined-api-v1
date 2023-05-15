using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.OAService
{
    public interface IOAService
    {
        Task<ServiceResponse<List<GetOADto>>> GetOA();
        Task<ServiceResponse<GetOADto>> GetOAById(int id);
        Task<ServiceResponse<GetOADto>> AddOA(AddOADto newOA);
        Task<ServiceResponse<GetOADto>> UpdateOA(UpdateOADto updatedOA);
        Task<ServiceResponse<List<GetOADto>>> DeleteOA(int id);
    }
}