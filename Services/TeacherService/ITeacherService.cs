using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.TeacherService
{
    public interface ITeacherService
    {
        /// <summary>
        /// Get list of all teachers.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetTeacherDto>>> GetTeacher();

        /// <summary>
        /// Get teacher by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetTeacherDto>> GetTeacherById(int id);

        /// <summary>
        /// Get self teacher.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<GetTeacherDto>> GetTeacherByToken();

        /// <summary>
        /// Add teaher.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetTeacherDto>> AddTeacher(AddTeacherDto request);

        /// <summary>
        /// Update teacher by teacherId (request.Id).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetTeacherDto>> UpdateTeacher(UpdateTeacherDto request);

        /// <summary>
        /// Delete teacher by given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetTeacherDto>>> DeleteTeacher(int id);

        /// <summary>
        /// Disable teacher's account by given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetTeacherDto>> DisableTeacher(int id);

        /// <summary>
        /// Enable teacher's account by given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetTeacherDto>> EnableTeacher(int id);

        /// <summary>
        /// Change teacher's password.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> ChangePasswordWithFirebaseId(string uid, ChangeUserPasswordDto password);

        /// <summary>
        /// Get teacher work types with given parameters.
        /// </summary>
        /// <param name="dbTeacher"></param>
        /// <param name="date"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        List<TeacherShiftResponseDto> GetTeacherWorkTypesWithHours(Teacher dbTeacher, DateTime date, TimeSpan fromTime, TimeSpan toTime);
    }
}