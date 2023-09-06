using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.ClassCancellationRequestService
{
    public interface IClassCancellationRequestService
    {
        Task<ServiceResponse<string>> AddClassCancellationRequest(int studyClassId); 
    }
}