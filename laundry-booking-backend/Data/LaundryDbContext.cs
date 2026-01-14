using Microsoft.EntityFrameworkCore;
using laundry_booking_backend.Models;

namespace laundry_booking_backend.Data;

public class LaundryDbContext : DbContext
{
    public LaundryDbContext(DbContextOptions<LaundryDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<BookedTimeSlot> BookedTimeSlots { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.BookedTimeSlots)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // BookedTimeSlot configuration
        modelBuilder.Entity<BookedTimeSlot>()
            .HasIndex(b => new { b.BookingDate, b.TimeSlotNumber })
            .IsUnique();
    }
}
