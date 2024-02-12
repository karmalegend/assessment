using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ParkingDbContext : DbContext
{
    public ParkingDbContext()
    {
    }

    public ParkingDbContext(DbContextOptions<ParkingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Garage> Garages { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ParkingSession> ParkingSessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Door>().OwnsOne(x => x.IpAddress);
        modelBuilder.Entity<Garage>().Property(x => x.ParkingSpotsAvailable).IsConcurrencyToken();
        modelBuilder.Entity<ParkingSession>().Property(x => x.SessionsState).HasConversion<string>();
    }
}
