using LogBook.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogBook.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Logbook> Logbooks => Set<Logbook>();
    public DbSet<LogEntry> LogEntries => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Logbook>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.HasIndex(e => e.Name);
        });

        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Duration).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasIndex(e => e.EntryDate);
            entity.HasOne(e => e.Logbook)
                  .WithMany(l => l.Entries)
                  .HasForeignKey(e => e.LogbookId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
