using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.CheckAvailableService
{
    public interface ICheckAvailableService
    {
        // Task<ServiceResponse<List<GetAvailableTeacherDto>>> GetAvailableTeacher(string fromTime, string toTime, string date, int classId);
        // Task<ServiceResponse<List<GetAvailableTimeDto>>> GetAvailableTime([FromQuery]int[] listOfStudentId, string date, int hour, int classId);
    }
}