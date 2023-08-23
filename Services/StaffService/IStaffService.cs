using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.StaffService
{
    public interface IStaffService
    {
        Task<ServiceResponse<List<StaffResponseDto>>> GetStaff();
        Task<ServiceResponse<StaffResponseDto>> GetStaffById(int id);
        Task<ServiceResponse<StaffResponseDto>> AddStaff(AddStaffRequestDto newStaff);
        Task<ServiceResponse<StaffResponseDto>> UpdateStaff(UpdateStaffRequestDto updatedStaff);
        Task<ServiceResponse<List<StaffResponseDto>>> DeleteStaff(int id);
        Task<ServiceResponse<StaffResponseDto>> DisableStaff(int id);
        Task<ServiceResponse<StaffResponseDto>> EnableStaff(int id);
    }
}