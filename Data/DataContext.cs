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

        // Enums
        // public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");
            });

            modelBuilder.Entity<AppointmentMember>(entity =>
            {
                entity.ToTable("AppointmentMember");
            });

            modelBuilder.Entity<CancellationRequest>(entity =>
            {
                entity.ToTable("CancellationRequest");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");
            });

            modelBuilder.Entity<CourseMember>(entity =>
            {
                entity.ToTable("CourseMember");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("Level");
            });

            modelBuilder.Entity<NewCourseRequest>(entity =>
            {
                entity.ToTable("NewCourseRequest");
            });

            modelBuilder.Entity<NewCourseSubjectRequest>(entity =>
            {
                entity.ToTable("NewCourseSubjectRequest");
            });

            modelBuilder.Entity<Parent>(entity =>
            {
                entity.ToTable("Parent");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");
            });

            modelBuilder.Entity<PreferredDay>(entity =>
            {
                entity.ToTable("PreferredDay");
            });

            modelBuilder.Entity<PreferredDayRequest>(entity =>
            {
                entity.ToTable("PreferredDayRequest");
            });

            modelBuilder.Entity<RegistrationRequest>(entity =>
            {
                entity.ToTable("RegistrationRequest");

                // entity.Property(e => e.id).HasColumnName("ID");

                // entity.Property(e => e.section)
                //     .IsRequired()
                //     .HasMaxLength(255);

                // entity.Property(e => e.type)
                //     .IsRequired()
                //     .HasMaxLength(255);

                // entity.Property(e => e.paymentType);

                entity.Property(e => e.date).HasColumnType("datetime");


                entity.HasMany(e => e.payment)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasMany(e => e.newCourseRequests)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasMany(e => e.registrationRequestMembers)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasOne(e => e.studentAddingRequest)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey<StudentAddingRequest>(e => e.registrationRequestId);

                entity.HasMany(e => e.preferredDayRequests)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);
            });

            modelBuilder.Entity<RegistrationRequestMember>(entity =>
            {
                entity.ToTable("RegistrationRequestMember");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staff");
            });

            modelBuilder.Entity<StaffNotification>(entity =>
            {
                entity.ToTable("StaffNotification");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");
            });

            modelBuilder.Entity<StudentAddingRequest>(entity =>
            {
                entity.ToTable("StudentAddingRequest");
            });

            modelBuilder.Entity<StudentAdditionalFile>(entity =>
            {
                entity.ToTable("StudentAdditionalFile");
            });

            modelBuilder.Entity<StudentAttendance>(entity =>
            {
                entity.ToTable("StudentAttendance");
            });

            modelBuilder.Entity<StudentNotification>(entity =>
            {
                entity.ToTable("StudentNotification");
            });

            modelBuilder.Entity<StudentReport>(entity =>
            {
                entity.ToTable("StudentReport");
            });

            modelBuilder.Entity<StudyClass>(entity =>
            {
                entity.ToTable("StudyClass");
            });

            modelBuilder.Entity<StudyCourse>(entity =>
            {
                entity.ToTable("StudyCourse");
            });

            modelBuilder.Entity<StudyCourseHistory>(entity =>
            {
                entity.ToTable("StudyCourseHistory");
            });

            modelBuilder.Entity<StudySubject>(entity =>
            {
                entity.ToTable("StudySubject");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");
            });

            modelBuilder.Entity<TeacherNotification>(entity =>
            {
                entity.ToTable("TeacherNotification");
            });

            modelBuilder.Entity<WorkTime>(entity =>
            {
                entity.ToTable("WorkTime");
            });
        }
    }
}