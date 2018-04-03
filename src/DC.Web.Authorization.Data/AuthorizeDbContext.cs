using DC.Web.Authorization.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DC.Web.Authorization.Data
{
    public class AuthorizeDbContext : DbContext
    {
        public AuthorizeDbContext(DbContextOptions<AuthorizeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Permission>().ToTable("Permission");
            modelBuilder.Entity<RolePermission>().ToTable("RolePermission");

            modelBuilder.Entity<Role>()
                .HasKey(c => c.RoleId);

            modelBuilder.Entity<Permission>()
                .HasKey(c => c.PermissionId);
            modelBuilder.Entity<RolePermission>()
                    .HasKey(c => new { c.RoleId, c.PermissionId });
        }
    }
}