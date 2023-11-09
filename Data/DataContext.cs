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
        public virtual DbSet<AppointmentHistory> AppointmentHistories { get; set; }
        public virtual DbSet<AppointmentMember> AppointmentMembers { get; set; }
        public virtual DbSet<AppointmentSlot> AppointmentSlots { get; set; }
        public virtual DbSet<ClassCancellationRequest> ClassCancellationRequests { get; set; }
        public virtual DbSet<RegistrationRequestComment> RegistrationRequestComments { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<NewCourseRequest> NewCourseRequests { get; set; }
        public virtual DbSet<NewCourseSubjectRequest> NewCourseSubjectRequests { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<NewCoursePreferredDayRequest> NewCoursePreferredDayRequests { get; set; }
        public virtual DbSet<ProfilePicture> ProfilePictures { get; set; }
        public virtual DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public virtual DbSet<RegistrationRequestPaymentFile> RegistrationRequestPaymentFiles { get; set; }
        public virtual DbSet<RegistrationRequestMember> RegistrationRequestMembers { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffNotification> StaffNotifications { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentAddingRequest> StudentAddingRequests { get; set; }
        public virtual DbSet<StudentAddingSubjectRequest> StudentAddingSubjectRequests { get; set; }
        public virtual DbSet<StudentAdditionalFile> StudentAdditionalFiles { get; set; }
        public virtual DbSet<StudentAttendance> StudentAttendances { get; set; }
        public virtual DbSet<StudentNotification> StudentNotifications { get; set; }
        public virtual DbSet<StudentReport> StudentReports { get; set; }
        public virtual DbSet<StudyClass> StudyClasses { get; set; }
        public virtual DbSet<StudyCourse> StudyCourses { get; set; }
        public virtual DbSet<StudyCourseHistory> StudyCourseHistories { get; set; }
        public virtual DbSet<StudySubject> StudySubjects { get; set; }
        public virtual DbSet<StudySubjectMember> StudySubjectMember { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<TeacherNotification> TeacherNotifications { get; set; }
        public virtual DbSet<TeacherShift> TeacherShifts { get; set; }
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

                entity.HasMany(e => e.AppointmentHistories)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasMany(e => e.AppointmentMembers)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasMany(e => e.AppointmentSlots)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasOne(e => e.Staff)
                    .WithMany(e => e.Appointments)
                    .HasForeignKey(e => e.CreatedByStaffId);

                entity.HasMany(e => e.TeacherNotifications)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId);
            });

            modelBuilder.Entity<AppointmentHistory>(entity =>
            {
                entity.ToTable("AppointmentHistory");

                entity.HasOne(e => e.Appointment)
                    .WithMany(e => e.AppointmentHistories)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasOne(e => e.Staff)
                    .WithMany(e => e.AppointmentHistories)
                    .HasForeignKey(e => e.StaffId);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.AppointmentHistories)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasOne(e => e.AppointmentSlot)
                    .WithMany(e => e.AppointmentHistories)
                    .HasForeignKey(e => e.AppointmentSlotId);
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

            modelBuilder.Entity<AppointmentSlot>(entity =>
            {
                entity.ToTable("AppointmentSlot");

                entity.HasOne(e => e.Appointment)
                    .WithMany(e => e.AppointmentSlots)
                    .HasForeignKey(e => e.AppointmentId);

                entity.HasMany(e => e.AppointmentHistories)
                    .WithOne(e => e.AppointmentSlot)
                    .HasForeignKey(e => e.AppointmentSlotId);

                entity.HasOne(e => e.Schedule)
                    .WithOne(e => e.AppointmentSlot)
                    .HasForeignKey<AppointmentSlot>(e => e.ScheduleId);
            });

            modelBuilder.Entity<ClassCancellationRequest>(entity =>
            {
                entity.ToTable("ClassCancellationRequest");

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.ClassCancellationRequests)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.ClassCancellationRequests)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.ClassCancellationRequests)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasOne(e => e.StudySubject)
                    .WithMany(e => e.ClassCancellationRequests)
                    .HasForeignKey(e => e.StudySubjectId);

                entity.HasOne(e => e.StudyClass)
                    .WithMany(e => e.ClassCancellationRequests)
                    .HasForeignKey(e => e.StudyClassId);

                entity.HasMany(e => e.StaffNotifications)
                    .WithOne(e => e.CancellationRequest)
                    .HasForeignKey(e => e.CancellationRequestId);
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

                entity.HasOne(e => e.StudyCourse)
                    .WithOne(e => e.NewCourseRequest)
                    .HasForeignKey<NewCourseRequest>(e => e.StudyCourseId);
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

            modelBuilder.Entity<NewCoursePreferredDayRequest>(entity =>
            {
                entity.ToTable("NewCoursePreferredDayRequest");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.NewCoursePreferredDayRequests)
                    .HasForeignKey(e => e.RegistrationRequestId);
            });

            modelBuilder.Entity<Parent>(entity =>
            {
                entity.ToTable("Parent");

                entity.HasOne(e => e.Student)
                    .WithOne(e => e.Parent)
                    .HasForeignKey<Parent>(e => e.StudentId);
            });

            modelBuilder.Entity<ProfilePicture>(entity =>
            {
                entity.ToTable("ProfilePicture");

                entity.HasOne(e => e.Student)
                    .WithOne(e => e.ProfilePicture)
                    .HasForeignKey<ProfilePicture>(e => e.StudentId);
            });

            modelBuilder.Entity<RegistrationRequest>(entity =>
            {
                entity.ToTable("RegistrationRequest");

                entity.HasMany(e => e.StudentAddingRequest)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.NewCourseRequests)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.RegistrationRequestMembers)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.NewCoursePreferredDayRequests)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.RegistrationRequestPaymentFiles)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.RegistrationRequestComments)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasMany(e => e.StaffNotifications)
                    .WithOne(e => e.RegistrationRequest)
                    .HasForeignKey(e => e.RegistrationRequestId);
            });

            modelBuilder.Entity<RegistrationRequestComment>(entity =>
            {
                entity.ToTable("RegistrationRequestComment");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.RegistrationRequestComments)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasOne(e => e.Staff)
                    .WithMany(e => e.RegistrationRequestComments)
                    .HasForeignKey(e => e.StaffId);
            });

            modelBuilder.Entity<RegistrationRequestPaymentFile>(entity =>
            {
                entity.ToTable("RegistrationRequestPaymentFile");

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.RegistrationRequestPaymentFiles)
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

                entity.HasOne(e => e.AppointmentSlot)
                    .WithOne(e => e.Schedule)
                    .HasForeignKey<AppointmentSlot>(e => e.ScheduleId);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.HasMany(e => e.Appointments)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.CreatedByStaffId);

                entity.HasMany(e => e.AppointmentHistories)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId);

                entity.HasMany(e => e.StaffNotifications)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId);

                entity.HasMany(e => e.StudyCourseHistories)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId);

                entity.HasMany(e => e.RegistrationRequestComments)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId);
            });

            modelBuilder.Entity<StaffNotification>(entity =>
            {
                entity.ToTable("StaffNotification");

                entity.HasOne(e => e.Staff)
                    .WithMany(e => e.StaffNotifications)
                    .HasForeignKey(e => e.StaffId);

                entity.HasOne(e => e.RegistrationRequest)
                    .WithMany(e => e.StaffNotifications)
                    .HasForeignKey(e => e.RegistrationRequestId);

                entity.HasOne(e => e.CancellationRequest)
                    .WithMany(e => e.StaffNotifications)
                    .HasForeignKey(e => e.CancellationRequestId);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.HasOne(e => e.ProfilePicture)
                    .WithOne(e => e.Student)
                    .HasForeignKey<ProfilePicture>(e => e.StudentId);

                entity.HasOne(e => e.Parent)
                    .WithOne(e => e.Student)
                    .HasForeignKey<Parent>(e => e.StudentId);

                entity.HasOne(e => e.Address)
                    .WithOne(e => e.Student)
                    .HasForeignKey<Address>(e => e.StudentId);

                entity.HasMany(e => e.Attendances)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.AdditionalFiles)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.RegistrationRequestMembers)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.StudySubjectMember)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.StudyCourseHistories)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId);

                entity.HasMany(e => e.ClassCancellationRequests)
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

            modelBuilder.Entity<StudentAddingSubjectRequest>(entity =>
            {
                entity.ToTable("StudentAddingSubjectRequest");

                entity.HasOne(e => e.StudentAddingRequest)
                    .WithMany(e => e.StudentAddingSubjectRequests)
                    .HasForeignKey(e => e.StudentAddingRequestId);

                entity.HasOne(e => e.StudySubject)
                    .WithMany(e => e.StudentAddingSubjectRequests)
                    .HasForeignKey(e => e.StudySubjectId);
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
                    .WithMany(e => e.Attendances)
                    .HasForeignKey(e => e.StudentId);

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

                entity.HasOne(e => e.StudySubjectMember)
                    .WithMany(e => e.StudentReports)
                    .HasForeignKey(e => e.StudySubjectMemberId);

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

                entity.HasMany(e => e.StudyCourseHistories)
                    .WithOne(e => e.StudyClass)
                    .HasForeignKey(e => e.StudyClassId);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.StudyClasses)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.TeacherShifts)
                    .WithOne(e => e.StudyClass)
                    .HasForeignKey(e => e.StudyClassId);

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.StudyClasses)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.Attendances)
                    .WithOne(e => e.StudyClass)
                    .HasForeignKey(e => e.StudyClassId);

                entity.HasMany(e => e.ClassCancellationRequests)
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

                entity.HasMany(e => e.StudyCourseHistories)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.StudentNotifications)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.TeacherNotifications)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.ClassCancellationRequests)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasMany(e => e.StudyClasses)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasOne(e => e.NewCourseRequest)
                    .WithOne(e => e.StudyCourse)
                    .HasForeignKey<NewCourseRequest>(e => e.StudyCourseId);
            });

            modelBuilder.Entity<StudyCourseHistory>(entity =>
            {
                entity.ToTable("StudyCourseHistory");

                entity.HasOne(e => e.StudyCourse)
                    .WithMany(e => e.StudyCourseHistories)
                    .HasForeignKey(e => e.StudyCourseId);

                entity.HasOne(e => e.Staff)
                    .WithMany(e => e.StudyCourseHistories)
                    .HasForeignKey(e => e.StaffId);

                entity.HasOne(e => e.StudyClass)
                    .WithMany(e => e.StudyCourseHistories)
                    .HasForeignKey(e => e.StudyClassId);
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

                entity.HasMany(e => e.StudySubjectMember)
                    .WithOne(e => e.StudySubject)
                    .HasForeignKey(e => e.StudySubjectId);

                entity.HasMany(e => e.StudyClasses)
                    .WithOne(e => e.StudySubject)
                    .HasForeignKey(e => e.StudySubjectId);

                entity.HasMany(e => e.ClassCancellationRequests)
                    .WithOne(e => e.StudySubject)
                    .HasForeignKey(e => e.StudySubjectId);
            });

            modelBuilder.Entity<StudySubjectMember>(entity =>
            {
                entity.ToTable("StudySubjectMember");

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.StudySubjectMember)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.StudySubject)
                    .WithMany(e => e.StudySubjectMember)
                    .HasForeignKey(e => e.StudySubjectId);

                entity.HasMany(e => e.StudentReports)
                    .WithOne(e => e.StudySubjectMember)
                    .HasForeignKey(e => e.StudySubjectMemberId);
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

                entity.HasMany(e => e.AppointmentHistories)
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

                entity.HasMany(e => e.TeacherShifts)
                    .WithOne(e => e.Teacher)
                    .HasForeignKey(e => e.TeacherId);

                entity.HasMany(e => e.ClassCancellationRequests)
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

            modelBuilder.Entity<TeacherShift>(entity =>
            {
                entity.ToTable("TeacherShift");

                entity.HasOne(e => e.Teacher)
                     .WithMany(e => e.TeacherShifts)
                     .HasForeignKey(e => e.TeacherId);

                entity.HasOne(e => e.StudyClass)
                    .WithMany(e => e.TeacherShifts)
                    .HasForeignKey(e => e.StudyClassId);
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