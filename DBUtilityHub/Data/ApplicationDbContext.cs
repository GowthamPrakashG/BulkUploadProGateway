
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
            
        }

    }
}
