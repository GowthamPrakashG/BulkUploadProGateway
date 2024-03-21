
using DBUtilityHub.Models;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<ScreenEntity> ScreenEntity { get; set; }
        public DbSet<RoleScreenMapping> RoleScreenMapping { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity
                {
                    Id = 1,
                    RoleName = "Super Admin"
                 
                });
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = 1,
                    Name = "Super Admin User",
                    RoleId = 1,
                    Email = "superadminuser@datayaan.com",
                    Password = new PasswordHasher<UserEntity>().HashPassword(null, "Datayaan@123"),
                    Phonenumber = "",
                    Gender = "",
                    Status = true
                });
        }

    }
}
