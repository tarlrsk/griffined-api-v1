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

        // Models
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<AppointmentMember> AppointmentMembers { get; set; }
        public virtual DbSet<CancellationRequest> CancellationRequests { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.HasOne(e => e.Student)
                    .WithOne(e => e.Address)
                    .HasForeignKey<Address>(e => e.StudentId);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");

                entity.HasMany(e => e.AppointmentMembers)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasMany(e => e.Schedules)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasMany(e => e.TeacherNotifications)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId);
            });

            modelBuilder.Entity<AppointmentMember>(entity =>
            {
                entity.ToTable("AppointmentMember");

                entity.HasOne(e => e.Appointment)
                    .WithMany(e => e.AppointmentMembers)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.AppointmentMembers)
                    .HasForeignKey(e => e.TeacherId);
            });

            modelBuilder.Entity<CancellationRequest>(entity =>
            {
                entity.ToTable("CancellationRequest");

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.CancellationRequests)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.CancellationRequests)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.CancellationRequests)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasOne(e => e.StudyClass)
                    .WithMany(e => e.CancellationRequests)
                    .HasForeignKey(e => e.StudyClassId);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.Comments)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasOne(e => e.Staff)
                    .WithMany(e => e.Comments)
                    .HasForeignKey(e => e.StaffId);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.HasMany(e => e.Subjects)
                    .WithOne(e => e.Course)
                    .HasForeignKey(e => e.CourseId);

                entity.HasMany(e => e.StudyCourses)
                    .WithOne(e => e.Course)
                    .HasForeignKey(e => e.CourseId);

                entity.HasMany(e => e.Levels)
                    .WithOne(e => e.Course)
                    .HasForeignKey(e => e.CourseId);

                entity.HasMany(e => e.NewCourseRequests)
                    .WithOne(e => e.Course)
                    .HasForeignKey(e => e.CourseId);
            });

            modelBuilder.Entity<CourseMember>(entity =>
            {
                entity.ToTable("CourseMember");

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.CourseMembers)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.StudySubject)
                    .WithMany(e => e.CourseMembers)
                    .HasForeignKey(e => e.StudySubjectId);

                entity.HasMany(e => e.StudentReports)
                    .WithOne(e => e.CourseMember)
                    .HasForeignKey(e => e.CourseMemberId);
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("Level");

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.Levels)
                    .HasForeignKey(e => e.CourseId);

                entity.HasMany(e => e.NewCourseRequests)
                    .WithOne(e => e.Level)
                    .HasForeignKey(e => e.LevelId);

                entity.HasMany(e => e.StudyCourses)
                    .WithOne(e => e.Level)
                    .HasForeignKey(e => e.LevelId);
            });

            modelBuilder.Entity<NewCourseRequest>(entity =>
            {
                entity.ToTable("NewCourseRequest");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.NewCourseRequests)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.NewCourseRequests)
                    .HasForeignKey(e => e.CourseId);

                entity.HasOne(e => e.Level)
                    .WithMany(e => e.NewCourseRequests)
                    .HasForeignKey(e => e.LevelId);

                entity.HasMany(e => e.NewCourseSubjectRequests)
                    .WithOne(e => e.NewCourseRequest)
                    .HasForeignKey(e => e.NewCourseRequestId);
            });

            modelBuilder.Entity<NewCourseSubjectRequest>(entity =>
            {
                entity.ToTable("NewCourseSubjectRequest");

                entity.HasOne(e => e.Subject)
                    .WithMany(e => e.NewCourseSubjectRequests)
                    .HasForeignKey(e => e.SubjectId);

                entity.HasOne(e => e.NewCourseRequest)
                    .WithMany(e => e.NewCourseSubjectRequests)
                    .HasForeignKey(e => e.NewCourseRequestId);
            });

            modelBuilder.Entity<Parent>(entity =>
            {
                entity.ToTable("Parent");

                entity.HasOne(e => e.Student)
                    .WithOne(e => e.Parent)
                    .HasForeignKey<Parent>(e => e.StudentId);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.Payment)
                    .HasForeignKey(e => e.RegistrationRequestId);
            });

            modelBuilder.Entity<PreferredDay>(entity =>
            {
                entity.ToTable("PreferredDay");

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.PreferredDays)
                    .HasForeignKey(e => e.StudyCourseId);
            });

            modelBuilder.Entity<PreferredDayRequest>(entity =>
            {
                entity.ToTable("PreferredDayRequest");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.PreferredDayRequests)
                    .HasForeignKey(e => e.RegistrationRequestId);
            });

            modelBuilder.Entity<RegistrationRequest>(entity =>
            {
                entity.ToTable("RegistrationRequest");

                entity.HasMany(e => e.StudentAddingRequest)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.Payment)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.NewCourseRequests)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.RegistrationRequestMembers)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.PreferredDayRequests)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.Comments)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);
            });

            modelBuilder.Entity<RegistrationRequestMember>(entity =>
            {
                entity.ToTable("RegistrationRequestMember");

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.RegistrationRequestMembers)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.RegistrationRequestMembers)
                    .HasForeignKey(e => e.RegistrationRequestId);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.HasOne(e => e.StudyClass)
                    .WithOne(e => e.Schedule)
                    .HasForeignKey<StudyClass>(e => e.ScheduleId);

                entity.HasOne(e => e.Appointment)
                    .WithMany(e => e.Schedules)
                    .HasForeignKey(e => e.AppointmentId);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.HasMany(e => e.StaffNotifications)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId);

                entity.HasMany(e => e.StudyCourseHistories)
                    .WithOne(e => e.ByStaff)
                    .HasForeignKey(e => e.StaffId);

                entity.HasMany(e => e.Comments)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId);
            });

            modelBuilder.Entity<StaffNotification>(entity =>
            {
                entity.ToTable("StaffNotification");

                entity.HasOne(e => e.Staff)
                    .WithMany(e => e.StaffNotifications)
                    .HasForeignKey(e => e.StaffId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.StaffNotifications)
                    .HasForeignKey(e => e.StudyCourseId);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.HasOne(e => e.Parent)
                    .WithOne(e => e.Student)
                    .HasForeignKey<Parent>(e => e.StudentId);

                entity.HasOne(e => e.Address)
                    .WithOne(e => e.Student)
                    .HasForeignKey<Address>(e => e.StudentId);

                entity.HasOne(e => e.Attendance)
                    .WithOne(e => e.Student)
                    .HasForeignKey<StudentAttendance>(e => e.StudentId);

                entity.HasMany(e => e.AdditionalFiles)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.RegistrationRequestMembers)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.CourseMembers)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.CancellationRequests)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.StudentNotifications)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);
            });

            modelBuilder.Entity<StudentAddingRequest>(entity =>
            {
                entity.ToTable("StudentAddingRequest");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.StudentAddingRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.StudentAddingRequests)
                    .HasForeignKey(e => e.StudyCourseId);
            });

            modelBuilder.Entity<StudentAdditionalFile>(entity =>
            {
                entity.ToTable("StudentAdditionalFile");

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.AdditionalFiles)
                    .HasForeignKey(e => e.StudentId);
            });

            modelBuilder.Entity<StudentAttendance>(entity =>
            {
                entity.ToTable("StudentAttendance");

                entity.HasOne(e => e.Student)
                    .WithOne(e => e.Attendance)
                    .HasForeignKey<StudentAttendance>(e => e.StudentId);

                entity.HasOne(e => e.StudyClass)
                    .WithMany(e => e.Attendances)
                    .HasForeignKey(e => e.StudyClassId);
            });

            modelBuilder.Entity<StudentNotification>(entity =>
            {
                entity.ToTable("StudentNotification");

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.StudentNotifications)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.StudentNotifications)
                    .HasForeignKey(e => e.StudyCourseId);
            });

            modelBuilder.Entity<StudentReport>(entity =>
            {
                entity.ToTable("StudentReport");

                entity.HasOne(e => e.CourseMember)
                    .WithMany(e => e.StudentReports)
                    .HasForeignKey(e => e.CourseMemberId);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.StudentReports)
                    .HasForeignKey(e => e.TeacherId);
            });

            modelBuilder.Entity<StudyClass>(entity =>
            {
                entity.ToTable("StudyClass");

                entity.HasOne(e => e.Schedule)
                    .WithOne(e => e.StudyClass)
                    .HasForeignKey<StudyClass>(e => e.ScheduleId);

                entity.HasOne(e => e.StudySubject)
                    .WithMany(e => e.StudyClasses)
                    .HasForeignKey(e => e.StudySubjectId);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.StudyClasses)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.Attendances)
                    .WithOne(e => e.StudyClass)
                    .HasForeignKey(e => e.StudyClassId);

                entity.HasMany(e => e.CancellationRequests)
                    .WithOne(e => e.StudyClass)
                    .HasForeignKey(e => e.StudyClassId);
            });

            modelBuilder.Entity<StudyCourse>(entity =>
            {
                entity.ToTable("StudyCourse");

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.StudyCourses)
                    .HasForeignKey(e => e.CourseId);

                entity.HasOne(e => e.Level)
                    .WithMany(e => e.StudyCourses)
                    .HasForeignKey(e => e.LevelId);

                entity.HasMany(e => e.StudentAddingRequests)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.StudySubjects)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.PreferredDays)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.StudyCourseHistories)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.StudentNotifications)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.TeacherNotifications)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.StaffNotifications)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.CancellationRequests)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);
            });

            modelBuilder.Entity<StudyCourseHistory>(entity =>
            {
                entity.ToTable("StudyCourseHistory");

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.StudyCourseHistories)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasOne(e => e.ByStaff)
                    .WithMany(e => e.StudyCourseHistories)
                    .HasForeignKey(e => e.StaffId);
            });

            modelBuilder.Entity<StudySubject>(entity =>
            {
                entity.ToTable("StudySubject");

                entity.HasOne(e => e.Subject)
                    .WithMany(e => e.StudySubjects)
                    .HasForeignKey(e => e.SubjectId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.StudySubjects)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.CourseMembers)
                    .WithOne(e => e.StudySubject)
                    .HasForeignKey(e => e.StudySubjectId);

                entity.HasMany(e => e.StudyClasses)
                    .WithOne(e => e.StudySubject)
                    .HasForeignKey(e => e.StudySubjectId);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.Subjects)
                    .HasForeignKey(e => e.CourseId);

                entity.HasMany(e => e.NewCourseSubjectRequests)
                    .WithOne(e => e.Subject)
                    .HasForeignKey(e => e.SubjectId);

                entity.HasMany(e => e.StudySubjects)
                    .WithOne(e => e.Subject)
                    .HasForeignKey(e => e.SubjectId);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");

                entity.HasMany(e => e.WorkTimes)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.StudyClasses)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.AppointmentMembers)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.StudentReports)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.TeacherNotifications)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.CancellationRequests)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId);
            });

            modelBuilder.Entity<TeacherNotification>(entity =>
            {
                entity.ToTable("TeacherNotification");

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.TeacherNotifications)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.TeacherNotifications)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasOne(e => e.Appointment)
                    .WithMany(e => e.TeacherNotifications)
                    .HasForeignKey(e => e.AppointmentId);
            });

            modelBuilder.Entity<WorkTime>(entity =>
            {
                entity.ToTable("WorkTime");

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.WorkTimes)
                    .HasForeignKey(e => e.TeacherId);
            });
        }
    }
}