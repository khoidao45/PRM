using Microsoft.EntityFrameworkCore;
using EVStation_basedRentalSystem.Services.CarAPI.Models;
using EVStation_basedRentalSystem.Services.CarAPI.utils.enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EVStation_basedRentalSystem.Services.CarAPI.Data
{
    public class CarDbContext : DbContext
    {
        public CarDbContext(DbContextOptions<CarDbContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarAvailability> CarAvailabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var carStateConverter = new EnumToStringConverter<CarState>();
            var carStatusConverter = new EnumToStringConverter<CarStatus>();

            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.BatteryCapacity).HasPrecision(10, 2);
                entity.Property(c => c.CurrentBatteryLevel).HasPrecision(10, 2);
                entity.Property(c => c.HourlyRate).HasPrecision(10, 2);
                entity.Property(c => c.DailyRate).HasPrecision(10, 2);

                // thêm conversion cho enum
                entity.Property(c => c.State).HasConversion(carStateConverter);
                entity.Property(c => c.Status).HasConversion(carStatusConverter);

            });
            modelBuilder.Entity<CarAvailability>(entity =>
            {
                entity.Property(a => a.Status)
                      .HasMaxLength(20)
                      .HasConversion<string>();
            });
        }
    }
}
