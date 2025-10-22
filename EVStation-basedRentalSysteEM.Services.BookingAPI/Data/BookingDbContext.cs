using EVStation_basedRentalSystem.Services.BookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EVStation_basedRentalSysteEM.Services.BookingAPI.utils.enums;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal precision
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            // Convert BookingStatus enum to string in DB
            var converter = new EnumToStringConverter<BookingStatus>();

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion(converter)
                .HasDefaultValue(BookingStatus.Pending); // default enum value, EF tự convert sang "Pending"
        }
    }
}
