using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Rpc;
using griffined_api.Dtos.AttendanceDtos;
using Microsoft.AspNetCore.Server.IIS.Core;

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

        public async Task<ServiceResponse<string>> UpdateStudentAttendance(int studyClassId, UpdateAttendanceRequestDto updateAttendanceRequest)
        {
            throw new NotImplementedException();
        }
    }
}