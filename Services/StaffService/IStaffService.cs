using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StaffService
{
    public interface IStaffService
    {
        Task<ServiceResponse<List<GetStaffDto>>> GetStaff();
        Task<ServiceResponse<GetStaffDto>> GetStaffById(int id);
        Task<ServiceResponse<GetStaffDto>> AddStaff(AddStaffDto newStaff);
        Task<ServiceResponse<GetStaffDto>> UpdateStaff(UpdateStaffDto updatedStaff);
        Task<ServiceResponse<List<GetStaffDto>>> DeleteStaff(int id);
        Task<ServiceResponse<GetStaffDto>> DisableStaff(int id);
        Task<ServiceResponse<GetStaffDto>> EnableStaff(int id);
    }
}