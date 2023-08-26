using Domain.Entities;

namespace Infrastructure.Persistence.DataSeed;

public static class SeedSpecies
{
    public static List<Species> Seed()
    {
        return new()
        {
            new Species() { Id = 1, Name = "Cachorro" },
            new Species() { Id = 2, Name = "Gato" }
        };
    }
}