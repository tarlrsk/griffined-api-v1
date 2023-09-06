using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.ClassCancellationRequestDto;

namespace griffined_api.Services.ClassCancellationRequestService
{
    public interface IClassCancellationRequestService
    {
        Task<ServiceResponse<string>> AddClassCancellationRequest(int studyClassId); 
        Task<ServiceResponse<List<ClassCancellationRequestResponseDto>>> ListAllClassCancellationRequest();
    }
}