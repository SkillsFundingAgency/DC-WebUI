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

        public DbSet<Feature> Features { get; set; }

        public DbSet<RoleFeature> RoleFeatures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Feature>().ToTable("Feature");
            modelBuilder.Entity<RoleFeature>().ToTable("RoleFeature");

            modelBuilder.Entity<Role>()
                .HasKey(c => c.RoleId);

            modelBuilder.Entity<Feature>()
                .HasKey(c => c.FeatureId);
            modelBuilder.Entity<RoleFeature>()
                    .HasKey(c => new { c.RoleId, c.FeatureId });
        }
    }
}