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
        /// Add firebase id to existing teacher by teacherId.
        /// </summary>
        Task AddTeacherFirebaseId();

        /// <summary>
        /// Delete firebase authentication records.
        /// </summary>
        /// <returns></returns>
        Task DeleteFirebaseAuthentication();
    }
}