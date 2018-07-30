using DC.Web.Ui.Services.ViewModels;
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

        public virtual DbSet<AppLogViewModel> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppLogViewModel>(entity =>
            {
                entity.Property(e => e.Level).HasMaxLength(128);

                entity.Property(e => e.TimeStampUtc)
                    .HasColumnName("TimeStampUTC")
                    .HasColumnType("datetime");
            });
        }
    }
}