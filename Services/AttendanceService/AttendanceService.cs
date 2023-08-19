using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace griffined_api.Services.AttendanceService
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public AttendanceService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<ServiceResponse<string>> UpdateStudentAttendance()
        {
            throw new NotImplementedException();
        }
    }
}