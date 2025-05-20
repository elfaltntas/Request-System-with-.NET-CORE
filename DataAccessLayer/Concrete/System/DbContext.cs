using DataAccessLayer.DBConfig;

using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;


using Entities.Configurations;

namespace DataAccessLayer.Concrete.System
{

    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) :
            base(options)
        {
        }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitPersonel> UnitPersonels { get; set; }
        public DbSet<Personel> Personels { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestProcess> RequestProcesses { get; set; }
        public DbSet<PersonelRoles> PersonelRoles { get; set; }
 
        public DbSet<Roles> Roles { get; set; }

        //public DbSet<Roles> Roles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UnitConfig());
            modelBuilder.ApplyConfiguration(new PersonelConfig());
            modelBuilder.ApplyConfiguration(new RolesConfig());
            modelBuilder.ApplyConfiguration(new UnitPersonelConfig());
            modelBuilder.ApplyConfiguration(new RequestConfig());
            modelBuilder.ApplyConfiguration(new RequestProcessConfig());
            modelBuilder.ApplyConfiguration(new UsersConfig());
            modelBuilder.ApplyConfiguration(new PersonelRolesConfig());
            modelBuilder.ApplyConfiguration(new RolesConfig());
        
        
        }
        //public IQueryable<RequestProcess> RequestProcessesNoTracking => RequestProcesses.AsNoTracking();
    }

}

