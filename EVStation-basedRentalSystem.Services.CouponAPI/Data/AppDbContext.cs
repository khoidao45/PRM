using EVStation_basedRentalSystem.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<AdminProfile> AdminProfiles { get; set; }
    public DbSet<StaffProfile> StaffProfiles { get; set; }
    public DbSet<RenterProfile> RenterProfiles { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.AdminProfile)
            .WithOne(a => a.User)
            .HasForeignKey<AdminProfile>(a => a.Id);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.StaffProfile)
            .WithOne(s => s.User)
            .HasForeignKey<StaffProfile>(s => s.Id);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.RenterProfile)
            .WithOne(r => r.User)
            .HasForeignKey<RenterProfile>(r => r.Id);
    }
}
