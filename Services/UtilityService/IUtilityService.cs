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

        /// <summary>
        /// Update study class number.
        /// </summary>
        /// <returns></returns>
        void UpdateStudyClassNumber();

        /// <summary>
        /// Update study course status.
        /// </summary>
        /// <returns></returns>
        Task UpdateStudyCourseStatus();

        /// <summary>
        /// Update student status.
        /// </summary>
        /// <returns></returns>
        Task UpdateStudentExpiryDate();

        /// <summary>
        /// Add student attendence to the study classes that do not have any attendence records.
        /// </summary>
        /// <returns></returns>
        Task AddStudentAttendence();
    }
}