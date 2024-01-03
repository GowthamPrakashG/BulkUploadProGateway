
using DBUtilityHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;


namespace DBUtilityHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<TableMetaDataEntity> TableMetaDataEntity { get; set; }
        public DbSet<ColumnMetaDataEntity> ColumnMetaDataEntity { get; set; }
        public DbSet<LogParent> LogParents { get; set; }
        public DbSet<LogChild> LogChilds { get; set; }

        public DbSet<RoleEntity> RoleEntity { get; set; }

        public DbSet<UserEntity> UserEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity
                {
                    Id = 1,
                    RoleName = "Admin"
                 
                },
                new RoleEntity
                {
                    Id = 2,
                    RoleName = "Developer"

                },
                new RoleEntity
                {
                    Id = 3,
                    RoleName = "Tester"

                });
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = 1,
                    Name = "SuperUser",
                    RoleId = 1,
                    Email = "superuser@datayaan.com",
                    Password = "Datayaan@123",
                    Phonenumber = "9876543210",
                    Gender = "",
                    Status = true

                });
        }

    }
}
