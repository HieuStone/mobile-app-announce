using CallAlert.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CallAlert.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<WatchNumber> WatchNumbers => Set<WatchNumber>();
    public DbSet<CallEvent> CallEvents => Set<CallEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
        });

        // Cấu hình WatchNumber
        modelBuilder.Entity<WatchNumber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.WatchNumbers)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Label).HasMaxLength(255);
        });

        // Cấu hình CallEvent
        modelBuilder.Entity<CallEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.CallEvents)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.CallerNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CallStatus).HasMaxLength(50);
            entity.Property(e => e.DeviceId).HasMaxLength(255);
        });
    }
}


