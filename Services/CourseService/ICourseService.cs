using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.CourseService
{
    public interface ICourseService
    {
        Task<ServiceResponse<List<CourseResponseDto>>> ListAllCourseSubjectLevel();
    }
}