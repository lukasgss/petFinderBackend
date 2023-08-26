using Domain.Entities;

namespace Infrastructure.Persistence.DataSeed;

public static class SeedBreeds
{
    public static List<Breed> Seed()
    {
        return new()
        {
            new Breed() { Id = 1, Name = "Border Collie", SpeciesId = 1 },
            new Breed() { Id = 2, Name = "Pastor Alemão", SpeciesId = 1 },
            new Breed() { Id = 3, Name = "Pug", SpeciesId = 1 },
            new Breed() { Id = 4, Name = "Dachshund", SpeciesId = 1 },
            new Breed() { Id = 5, Name = "Golden", SpeciesId = 1 },
            new Breed() { Id = 6, Name = "Siamês", SpeciesId = 2 }
        };
    }
}