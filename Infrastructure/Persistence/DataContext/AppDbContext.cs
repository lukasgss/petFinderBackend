using Domain.Entities;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataSeed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.DataContext;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Breed> Breeds { get; set; } = null!;
    public DbSet<Color> Colors { get; set; } = null!;
    public DbSet<Species> Species { get; set; } = null!;
    public DbSet<MissingAlert> MissingAlerts { get; set; } = null!;
    public DbSet<AdoptionAlert> AdoptionAlerts { get; set; } = null!;
    public DbSet<UserMessage> UserMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Breed>().HasData(SeedBreeds.Seed());
        builder.Entity<Color>().HasData(SeedColors.Seed());
        builder.Entity<Species>().HasData(SeedSpecies.Seed());
    }
}