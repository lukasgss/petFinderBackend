using System.Collections.Generic;
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
            Name = "Text",
            Species = species,
            SpeciesId = species.Id
        };
    }

    public static List<Breed> GenerateListOfBreeds()
    {
        return new List<Breed>()
        {
            GenerateBreed()
        };
    }
}