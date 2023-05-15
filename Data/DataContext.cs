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

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Parent> Parents => Set<Parent>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<WorkTime> WorkTimes => Set<WorkTime>();
        public DbSet<EP> EPs => Set<EP>();
        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<EA> EAs => Set<EA>();
        public DbSet<OA> OAs => Set<OA>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentFile> PaymentFiles => Set<PaymentFile>();
        public DbSet<StudentAdditionalFiles> StudentAdditionalFiles => Set<StudentAdditionalFiles>();
        public DbSet<PrivateRegistrationRequest> PrivateRegistrationRequests => Set<PrivateRegistrationRequest>();
        public DbSet<PrivateRegistrationRequestInfo> PrivateRegistrationRequestInfos => Set<PrivateRegistrationRequestInfo>();
        public DbSet<PreferredDay> PreferredDays => Set<PreferredDay>();
        public DbSet<PrivateCourse> PrivateCourses => Set<PrivateCourse>();
        public DbSet<PrivateClass> PrivateClasses => Set<PrivateClass>();
        public DbSet<StudentPrivateClass> StudentPrivateClasses => Set<StudentPrivateClass>();
        public DbSet<TeacherPrivateClass> TeacherPrivateClasses => Set<TeacherPrivateClass>();
        public DbSet<TeacherLeavingRequest> TeacherLeavingRequests => Set<TeacherLeavingRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasMany(s => s.privateRegistrationRequests)
                .WithMany(r => r.students)
                .UsingEntity(sr => sr.ToTable("StudentRequests"));

            modelBuilder.Entity<PrivateRegistrationRequest>()
                .HasMany(r => r.privateRegistrationRequestInfos)
                .WithOne(i => i.privateRegistrationRequest)
                .HasForeignKey(i => i.requestId);

            modelBuilder.Entity<PrivateRegistrationRequestInfo>()
                .HasMany(i => i.preferredDays)
                .WithOne(pd => pd.privateReqInfo)
                .HasForeignKey(pd => pd.privateReqInfoId);

        }
    }
}