using DC.Web.Authorization.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DC.Web.Authorization.Data
{
    public class AuthorizeDbContext : DbContext
    {
        private bool _disposed = false;

        public AuthorizeDbContext()
        {
        }

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
                .HasKey(c => c.Id);

            modelBuilder.Entity<Feature>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<RoleFeature>()
                    .HasKey(c => new { c.RoleId, c.FeatureId });

            modelBuilder.Entity<RoleFeature>().Ignore(x => x.Id);
        }
    }
}