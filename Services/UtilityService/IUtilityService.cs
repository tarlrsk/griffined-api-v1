namespace griffined_api.Services.UtilityService
{
    public interface IUtilityService
    {
        /// <summary>
        /// Add firebase id to existing student.
        /// </summary>
        Task AddStudentFirebaseId();

        /// <summary>
        /// Add student code to existing students.
        /// </summary>
        /// <returns></returns>
        Task AddStudentCode();

        /// <summary>
        /// Add firebase id to existing teachers.
        /// </summary>
        Task AddTeacherFirebaseId();

        /// <summary>
        /// Add firebase id to exisitng staff.
        /// </summary>
        /// <returns></returns>
        Task AddStaffFirebaseId();

        /// <summary>
        /// Delete firebase authentication records.
        /// </summary>
        /// <returns></returns>
        Task DeleteFirebaseAuthentication();
    }
}