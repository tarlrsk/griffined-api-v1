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

                entity.HasOne(e => e.student)
                    .WithOne(e => e.address)
                    .HasForeignKey<Address>(e => e.studentId);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");

                entity.HasMany(e => e.appointmentMembers)
                    .WithOne(e => e.appointment)
                    .HasForeignKey(e => e.appointmentId);

                entity.HasMany(e => e.schedules)
                    .WithOne(e => e.appointment)
                    .HasForeignKey(e => e.appointmentId);

                entity.HasMany(e => e.teacherNotifications)
                    .WithOne(e => e.appointment)
                    .HasForeignKey(e => e.appointmentId);
            });

            modelBuilder.Entity<AppointmentMember>(entity =>
            {
                entity.ToTable("AppointmentMember");

                entity.HasOne(e => e.appointment)
                    .WithMany(e => e.appointmentMembers)
                    .HasForeignKey(e => e.appointmentId);

                entity.HasOne(e => e.teacher)
                    .WithMany(e => e.appointmentMembers)
                    .HasForeignKey(e => e.teacherId);
            });

            modelBuilder.Entity<CancellationRequest>(entity =>
            {
                entity.ToTable("CancellationRequest");

                entity.HasOne(e => e.student)
                    .WithMany(e => e.cancellationRequests)
                    .HasForeignKey(e => e.studentId);

                entity.HasOne(e => e.teacher)
                    .WithMany(e => e.cancellationRequests)
                    .HasForeignKey(e => e.teacherId);

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.cancellationRequests)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasOne(e => e.studyClass)
                    .WithMany(e => e.cancellationRequests)
                    .HasForeignKey(e => e.studyClassId);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.HasOne(e => e.registrationRequest)
                    .WithMany(e => e.comments)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasOne(e => e.staff)
                    .WithMany(e => e.comments)
                    .HasForeignKey(e => e.staffId);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.HasMany(e => e.subjects)
                    .WithOne(e => e.course)
                    .HasForeignKey(e => e.courseId);

                entity.HasMany(e => e.studyCourses)
                    .WithOne(e => e.course)
                    .HasForeignKey(e => e.courseId);

                entity.HasMany(e => e.levels)
                    .WithOne(e => e.course)
                    .HasForeignKey(e => e.courseId);

                entity.HasMany(e => e.newCourseRequests)
                    .WithOne(e => e.course)
                    .HasForeignKey(e => e.courseId);
            });

            modelBuilder.Entity<CourseMember>(entity =>
            {
                entity.ToTable("CourseMember");

                entity.HasOne(e => e.student)
                    .WithMany(e => e.courseMembers)
                    .HasForeignKey(e => e.studentId);

                entity.HasOne(e => e.studySubject)
                    .WithMany(e => e.courseMembers)
                    .HasForeignKey(e => e.studySubjectId);

                entity.HasMany(e => e.studentReports)
                    .WithOne(e => e.courseMember)
                    .HasForeignKey(e => e.courseMemberId);
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("Level");

                entity.HasOne(e => e.course)
                    .WithMany(e => e.levels)
                    .HasForeignKey(e => e.courseId);

                entity.HasMany(e => e.newCourseRequests)
                    .WithOne(e => e.level)
                    .HasForeignKey(e => e.levelId);
            });

            modelBuilder.Entity<NewCourseRequest>(entity =>
            {
                entity.ToTable("NewCourseRequest");

                entity.HasOne(e => e.registrationRequest)
                    .WithMany(e => e.newCourseRequests)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasOne(e => e.course)
                    .WithMany(e => e.newCourseRequests)
                    .HasForeignKey(e => e.courseId);

                entity.HasOne(e => e.level)
                    .WithMany(e => e.newCourseRequests)
                    .HasForeignKey(e => e.levelId);

                entity.HasMany(e => e.newCourseSubjectRequests)
                    .WithOne(e => e.newCourseRequest)
                    .HasForeignKey(e => e.newCourseRequestId);
            });

            modelBuilder.Entity<NewCourseSubjectRequest>(entity =>
            {
                entity.ToTable("NewCourseSubjectRequest");

                entity.HasOne(e => e.subject)
                    .WithMany(e => e.newCourseSubjectRequests)
                    .HasForeignKey(e => e.subjectId);

                entity.HasOne(e => e.newCourseRequest)
                    .WithMany(e => e.newCourseSubjectRequests)
                    .HasForeignKey(e => e.newCourseRequestId);
            });

            modelBuilder.Entity<Parent>(entity =>
            {
                entity.ToTable("Parent");

                entity.HasOne(e => e.student)
                    .WithOne(e => e.parent)
                    .HasForeignKey<Parent>(e => e.studentId);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.HasOne(e => e.registrationRequest)
                    .WithMany(e => e.payment)
                    .HasForeignKey(e => e.registrationRequestId);
            });

            modelBuilder.Entity<PreferredDay>(entity =>
            {
                entity.ToTable("PreferredDay");

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.preferredDays)
                    .HasForeignKey(e => e.studyCourseId);
            });

            modelBuilder.Entity<PreferredDayRequest>(entity =>
            {
                entity.ToTable("PreferredDayRequest");

                entity.HasOne(e => e.registrationRequest)
                    .WithMany(e => e.preferredDayRequests)
                    .HasForeignKey(e => e.registrationRequestId);
            });

            modelBuilder.Entity<RegistrationRequest>(entity =>
            {
                entity.ToTable("RegistrationRequest");

                entity.HasMany(e => e.studentAddingRequest)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasMany(e => e.payment)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasMany(e => e.newCourseRequests)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasMany(e => e.registrationRequestMembers)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasMany(e => e.preferredDayRequests)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasMany(e => e.comments)
                    .WithOne(e => e.registrationRequest)
                    .HasForeignKey(e => e.registrationRequestId);
            });

            modelBuilder.Entity<RegistrationRequestMember>(entity =>
            {
                entity.ToTable("RegistrationRequestMember");

                entity.HasOne(e => e.student)
                    .WithMany(e => e.registrationRequestMembers)
                    .HasForeignKey(e => e.studentId);

                entity.HasOne(e => e.registrationRequest)
                    .WithMany(e => e.registrationRequestMembers)
                    .HasForeignKey(e => e.registrationRequestId);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.HasOne(e => e.studyClass)
                    .WithOne(e => e.schedule)
                    .HasForeignKey<StudyClass>(e => e.scheduleId);

                entity.HasOne(e => e.appointment)
                    .WithMany(e => e.schedules)
                    .HasForeignKey(e => e.appointmentId);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.HasMany(e => e.staffNotifications)
                    .WithOne(e => e.staff)
                    .HasForeignKey(e => e.staffId);

                entity.HasMany(e => e.studyCourseHistories)
                    .WithOne(e => e.byStaff)
                    .HasForeignKey(e => e.staffId);

                entity.HasMany(e => e.comments)
                    .WithOne(e => e.staff)
                    .HasForeignKey(e => e.staffId);
            });

            modelBuilder.Entity<StaffNotification>(entity =>
            {
                entity.ToTable("StaffNotification");

                entity.HasOne(e => e.staff)
                    .WithMany(e => e.staffNotifications)
                    .HasForeignKey(e => e.staffId);

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.staffNotifications)
                    .HasForeignKey(e => e.studyCourseId);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.HasOne(e => e.parent)
                    .WithOne(e => e.student)
                    .HasForeignKey<Parent>(e => e.studentId);

                entity.HasOne(e => e.address)
                    .WithOne(e => e.student)
                    .HasForeignKey<Address>(e => e.studentId);

                entity.HasOne(e => e.attendance)
                    .WithOne(e => e.student)
                    .HasForeignKey<StudentAttendance>(e => e.studentId);

                entity.HasMany(e => e.additionalFiles)
                    .WithOne(e => e.student)
                    .HasForeignKey(e => e.studentId);

                entity.HasMany(e => e.registrationRequestMembers)
                    .WithOne(e => e.student)
                    .HasForeignKey(e => e.studentId);

                entity.HasMany(e => e.courseMembers)
                    .WithOne(e => e.student)
                    .HasForeignKey(e => e.studentId);

                entity.HasMany(e => e.cancellationRequests)
                    .WithOne(e => e.student)
                    .HasForeignKey(e => e.studentId);

                entity.HasMany(e => e.studentNotifications)
                    .WithOne(e => e.student)
                    .HasForeignKey(e => e.studentId);
            });

            modelBuilder.Entity<StudentAddingRequest>(entity =>
            {
                entity.ToTable("StudentAddingRequest");

                entity.HasOne(e => e.registrationRequest)
                    .WithMany(e => e.studentAddingRequest)
                    .HasForeignKey(e => e.registrationRequestId);

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.studentAddingRequests)
                    .HasForeignKey(e => e.studyCourseId);
            });

            modelBuilder.Entity<StudentAdditionalFile>(entity =>
            {
                entity.ToTable("StudentAdditionalFile");

                entity.HasOne(e => e.student)
                    .WithMany(e => e.additionalFiles)
                    .HasForeignKey(e => e.studentId);
            });

            modelBuilder.Entity<StudentAttendance>(entity =>
            {
                entity.ToTable("StudentAttendance");

                entity.HasOne(e => e.student)
                    .WithOne(e => e.attendance)
                    .HasForeignKey<StudentAttendance>(e => e.studentId);

                entity.HasOne(e => e.studyClass)
                    .WithMany(e => e.attendances)
                    .HasForeignKey(e => e.studyClassId);
            });

            modelBuilder.Entity<StudentNotification>(entity =>
            {
                entity.ToTable("StudentNotification");

                entity.HasOne(e => e.student)
                    .WithMany(e => e.studentNotifications)
                    .HasForeignKey(e => e.studentId);

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.studentNotifications)
                    .HasForeignKey(e => e.studyCourseId);
            });

            modelBuilder.Entity<StudentReport>(entity =>
            {
                entity.ToTable("StudentReport");

                entity.HasOne(e => e.courseMember)
                    .WithMany(e => e.studentReports)
                    .HasForeignKey(e => e.courseMemberId);

                entity.HasOne(e => e.teacher)
                    .WithMany(e => e.studentReports)
                    .HasForeignKey(e => e.teacherId);
            });

            modelBuilder.Entity<StudyClass>(entity =>
            {
                entity.ToTable("StudyClass");

                entity.HasOne(e => e.schedule)
                    .WithOne(e => e.studyClass)
                    .HasForeignKey<StudyClass>(e => e.scheduleId);

                entity.HasOne(e => e.studySubject)
                    .WithMany(e => e.studyClasses)
                    .HasForeignKey(e => e.studySubjectId);

                entity.HasOne(e => e.teacher)
                    .WithMany(e => e.studyClasses)
                    .HasForeignKey(e => e.teacherId);

                entity.HasMany(e => e.attendances)
                    .WithOne(e => e.studyClass)
                    .HasForeignKey(e => e.studyClassId);

                entity.HasMany(e => e.cancellationRequests)
                    .WithOne(e => e.studyClass)
                    .HasForeignKey(e => e.studyClassId);
            });

            modelBuilder.Entity<StudyCourse>(entity =>
            {
                entity.ToTable("StudyCourse");

                entity.HasOne(e => e.course)
                    .WithMany(e => e.studyCourses)
                    .HasForeignKey(e => e.courseId);

                entity.HasMany(e => e.studentAddingRequests)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.studySubjects)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.preferredDays)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.studyCourseHistories)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.studentNotifications)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.teacherNotifications)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.staffNotifications)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.cancellationRequests)
                    .WithOne(e => e.studyCourse)
                    .HasForeignKey(e => e.studyCourseId);
            });

            modelBuilder.Entity<StudyCourseHistory>(entity =>
            {
                entity.ToTable("StudyCourseHistory");

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.studyCourseHistories)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasOne(e => e.byStaff)
                    .WithMany(e => e.studyCourseHistories)
                    .HasForeignKey(e => e.staffId);
            });

            modelBuilder.Entity<StudySubject>(entity =>
            {
                entity.ToTable("StudySubject");

                entity.HasOne(e => e.subject)
                    .WithMany(e => e.studySubjects)
                    .HasForeignKey(e => e.subjectId);

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.studySubjects)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasMany(e => e.courseMembers)
                    .WithOne(e => e.studySubject)
                    .HasForeignKey(e => e.studySubjectId);

                entity.HasMany(e => e.studyClasses)
                    .WithOne(e => e.studySubject)
                    .HasForeignKey(e => e.studySubjectId);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.HasOne(e => e.course)
                    .WithMany(e => e.subjects)
                    .HasForeignKey(e => e.courseId);

                entity.HasMany(e => e.newCourseSubjectRequests)
                    .WithOne(e => e.subject)
                    .HasForeignKey(e => e.subjectId);

                entity.HasMany(e => e.studySubjects)
                    .WithOne(e => e.subject)
                    .HasForeignKey(e => e.subjectId);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");

                entity.HasMany(e => e.workTimes)
                    .WithOne(e => e.teacher)
                    .HasForeignKey(e => e.teacherId);

                entity.HasMany(e => e.studyClasses)
                    .WithOne(e => e.teacher)
                    .HasForeignKey(e => e.teacherId);

                entity.HasMany(e => e.appointmentMembers)
                    .WithOne(e => e.teacher)
                    .HasForeignKey(e => e.teacherId);

                entity.HasMany(e => e.studentReports)
                    .WithOne(e => e.teacher)
                    .HasForeignKey(e => e.teacherId);

                entity.HasMany(e => e.teacherNotifications)
                    .WithOne(e => e.teacher)
                    .HasForeignKey(e => e.teacherId);

                entity.HasMany(e => e.cancellationRequests)
                    .WithOne(e => e.teacher)
                    .HasForeignKey(e => e.teacherId);
            });

            modelBuilder.Entity<TeacherNotification>(entity =>
            {
                entity.ToTable("TeacherNotification");

                entity.HasOne(e => e.teacher)
                    .WithMany(e => e.teacherNotifications)
                    .HasForeignKey(e => e.teacherId);

                entity.HasOne(e => e.studyCourse)
                    .WithMany(e => e.teacherNotifications)
                    .HasForeignKey(e => e.studyCourseId);

                entity.HasOne(e => e.appointment)
                    .WithMany(e => e.teacherNotifications)
                    .HasForeignKey(e => e.appointmentId);
            });

            modelBuilder.Entity<WorkTime>(entity =>
            {
                entity.ToTable("WorkTime");

                entity.HasOne(e => e.teacher)
                    .WithMany(e => e.workTimes)
                    .HasForeignKey(e => e.teacherId);
            });
        }
    }
}