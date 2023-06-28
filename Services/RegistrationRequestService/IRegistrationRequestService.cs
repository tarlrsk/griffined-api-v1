using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;

namespace griffined_api.Services.RegistrationRequestService
{
    public interface IRegistrationRequestService
    {
        Task<ServiceResponse<String>> AddNewRequestedCourses(NewCoursesRequestDto newCourses);
        // Task<ServiceResponse<List<GetAvailableTeacherDto>>> GetAvailableTeacher(string fromTime, string toTime, string date, int classId);
        // Task<ServiceResponse<List<GetAvailableTimeDto>>> GetAvailableTime([FromQuery]int[] listOfStudentId, string date, int hour, int classId);
    }
}