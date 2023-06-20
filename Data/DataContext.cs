using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable

namespace griffined_api.Data
{
    public class DataContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<AppointmentMember> AppointmentMembers { get; set; }
        public virtual DbSet<CancellationRequest> CancellationRequests { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseMember> CourseMembers { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<NewCourseRequest> NewCourseRequests { get; set; }
        public virtual DbSet<NewCourseSubjectRequest> NewCourseSubjectRequests { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<PreferredDay> PreferredDays { get; set; }
        public virtual DbSet<PreferredDayRequest> PreferredDayRequests { get; set; }
        public virtual DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public virtual DbSet<RegistrationRequestMember> RegistrationRequestMembers { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffNotification> StaffNotifications { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentAddingRequest> StudentAddingRequests { get; set; }
        public virtual DbSet<StudentAdditionalFile> StudentAdditionalFiles { get; set; }
        public virtual DbSet<StudentAttendance> StudentAttendances { get; set; }
        public virtual DbSet<StudentNotification> StudentNotifications { get; set; }
        public virtual DbSet<StudentReport> StudentReports { get; set; }
        public virtual DbSet<StudyClass> StudyClasses { get; set; }
        public virtual DbSet<StudyCourse> StudyCourses { get; set; }
        public virtual DbSet<StudyCourseHistory> StudyCourseHistories { get; set; }
        public virtual DbSet<StudySubject> StudySubjects { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<TeacherNotification> TeacherNotifications { get; set; }
        public virtual DbSet<WorkTime> WorkTimes { get; set; }
    }
}