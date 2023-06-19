using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Data
{
    public class DataContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentMember> AppointmentMembers => Set<AppointmentMember>();
        public DbSet<CancellationRequest> CancellationRequests => Set<CancellationRequest>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseMember> CourseMembers => Set<CourseMember>();
        public DbSet<Level> Levels => Set<Level>();
        public DbSet<NewCourseRequest> NewCourseRequests => Set<NewCourseRequest>();
        public DbSet<NewCourseSubjectRequest> NewCourseSubjectRequests => Set<NewCourseSubjectRequest>();
        public DbSet<Parent> Parents => Set<Parent>();
        public DbSet<PreferredDay> PreferredDays => Set<PreferredDay>();
        public DbSet<PreferredDayRequest> PreferredDayRequests => Set<PreferredDayRequest>();
        public DbSet<RegistrationRequest> RegistrationRequests => Set<RegistrationRequest>();
        public DbSet<RegistrationRequestMember> RegistrationRequestMembers => Set<RegistrationRequestMember>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Staff> Staff => Set<Staff>();
        public DbSet<StaffNotification> StaffNotifications => Set<StaffNotification>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentAddingRequest> StudentAddingRequests => Set<StudentAddingRequest>();
        public DbSet<StudentAdditionalFile> StudentAdditionalFiles => Set<StudentAdditionalFile>();
        public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();
        public DbSet<StudentNotification> StudentNotifications => Set<StudentNotification>();
        public DbSet<StudentReport> StudentReports => Set<StudentReport>();
        public DbSet<StudyClass> StudyClasses => Set<StudyClass>();
        public DbSet<StudyCourse> StudyCourses => Set<StudyCourse>();
        public DbSet<StudyCourseHistory> StudyCourseHistories => Set<StudyCourseHistory>();
        public DbSet<StudySubject> StudySubjects => Set<StudySubject>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<TeacherNotification> TeacherNotifications => Set<TeacherNotification>();
        public DbSet<WorkTime> WorkTimes => Set<WorkTime>();
    }
}