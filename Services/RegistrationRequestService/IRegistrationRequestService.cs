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
    }
}