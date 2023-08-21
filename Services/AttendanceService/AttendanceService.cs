using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Rpc;
using griffined_api.Dtos.AttendanceDtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace griffined_api.Services.AttendanceService
{
    public class AttendanceService : IAttendanceService
    {
        private readonly DataContext _context;
        public AttendanceService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<string>> UpdateStudentAttendance(int studyClassId, List<UpdateAttendanceRequestDto> updateAttendanceRequests)
        {
            var response = new ServiceResponse<string>();

            var dbClass = await _context.StudyClasses
                                    .Include(c => c.Attendances)
                                        .ThenInclude(a => a.Student)
                                    .FirstOrDefaultAsync(c => c.Id == studyClassId) ?? throw new NotFoundException($"Class with ID {studyClassId} not found.");

            foreach (var updateAttendanceRequest in updateAttendanceRequests)
            {
                var dbAttendance = dbClass.Attendances
                                    .FirstOrDefault(a => a.StudentId == updateAttendanceRequest.StudentId) ?? throw new NotFoundException($"No student found.");

                dbAttendance.Attendance = updateAttendanceRequest.Attendance;
            }

            bool allAttendancesSet = dbClass.Attendances.All(a => a.Attendance != Attendance.None);

            if (allAttendancesSet)
            {
                dbClass.Status = ClassStatus.Check;
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = "success";
            return response;
        }
    }
}