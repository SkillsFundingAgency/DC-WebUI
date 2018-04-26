using DC.Web.Ui.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace DC.Web.Ui.Services.AppLogs
{
    public class AppLogsContext : DbContext
    {
        public AppLogsContext()
        {
        }

        public AppLogsContext(DbContextOptions<AppLogsContext> options)
        : base(options)
        {
        }

        public virtual DbSet<AppLog> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppLog>(entity =>
            {
                entity.Property(e => e.Level).HasMaxLength(128);

                entity.Property(e => e.TimeStampUtc)
                    .HasColumnName("TimeStampUTC")
                    .HasColumnType("datetime");
            });
        }
    }
}