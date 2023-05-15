using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<ServiceResponse<List<GetScheduleDto>>> GetSchedule();
        Task<ServiceResponse<List<GetScheduleDto>>> GetScheduleByRequestId(int requestId);
        Task<ServiceResponse<GetScheduleDto>> AddSchedule(AddScheduleDto newSchedule);
        Task<ServiceResponse<GetSinglePrivateClassDto>> AddPrivateClass(AddSinglePrivateClassDto newClass);
        Task<ServiceResponse<List<GetSinglePrivateClassDto>>> AddListOfPrivateClass(List<AddSinglePrivateClassDto> newClasses);
        Task<ServiceResponse<UpdatePrivateClassDto>> UpdatePrivateClass(UpdatePrivateClassDto updatedClass);
        Task<ServiceResponse<List<UpdatePrivateClassDto>>> UpdateListOfPrivateClass(List<UpdatePrivateClassDto> updatedClasses);
        Task<ServiceResponse<List<GetScheduleDto>>> DeleteSchedule(int courseId);
        Task<ServiceResponse<List<GetScheduleDto>>> SoftDeleteSchedule(int courseId);
        Task<ServiceResponse<List<GetPrivateClassDto>>> DeletePrivateClass(int classId);
        Task<ServiceResponse<List<GetPrivateClassDto>>> DeleteListOfPrivateClass([FromQuery] int[] listOfClassId);
    }
}