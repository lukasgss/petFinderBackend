using Domain.Entities;

namespace Tests.EntityGenerators;

public static class BreedGenerator
{
    public static Breed GenerateBreed()
    {
        Species species = SpeciesGenerator.GenerateSpecies();
        
        return new Breed()
        {
            Id = 1,
            Name = "Border Collie",
            Species = species,
            SpeciesId = species.Id
        };
    }
}