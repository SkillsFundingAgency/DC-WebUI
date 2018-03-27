using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Settings.Models;
using Microsoft.EntityFrameworkCore;

namespace DC.Web.Ui.Services.AppLogs
{
    public class AppLogsContext : DbContext
    {
        public virtual DbSet<AppLog> Logs { get; set; }

        public AppLogsContext()
        { }
        public AppLogsContext(DbContextOptions options)
        : base(options)
        {
        }
      
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