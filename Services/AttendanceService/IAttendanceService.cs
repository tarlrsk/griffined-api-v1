using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.AttendanceService
{
    public interface IAttendanceService
    {
        Task<ServiceResponse<string>> UpdateStudentAttendance(int studyClassId, UpdateAttendanceRequestDto updateAttendanceRequest);
    }
}