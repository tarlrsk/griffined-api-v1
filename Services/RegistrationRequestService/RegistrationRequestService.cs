using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;

namespace griffined_api.Services.RegistrationRequestService
{
    public class RegistrationRequestService : IRegistrationRequestService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RegistrationRequestService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<ServiceResponse<string>> AddNewCourses(NewCoursesRequestDto newCourses)
        {
            throw new NotImplementedException();
        }
    }
}