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
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<WorkTime> WorkTimes => Set<WorkTime>();
        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<PaymentFile> PaymentFiles => Set<PaymentFile>();
        public DbSet<StudentAdditionalFiles> StudentAdditionalFiles => Set<StudentAdditionalFiles>();
        public DbSet<PreferredDay> PreferredDays => Set<PreferredDay>();
    }
}