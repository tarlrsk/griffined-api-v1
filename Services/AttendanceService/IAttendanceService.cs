namespace griffined_api.Services.AttendanceService
{
    public interface IAttendanceService
    {
        Task<ServiceResponse<AttendanceResponseDto>> GetClassAttendance(int studyClassId);
        Task<ServiceResponse<string>> UpdateStudentAttendance(int studyClassId, List<UpdateAttendanceRequestDto> updateAttendanceRequests);
    }
}