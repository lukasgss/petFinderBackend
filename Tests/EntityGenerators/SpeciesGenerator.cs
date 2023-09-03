using System.Collections.Generic;
using Domain.Entities;

namespace Tests.EntityGenerators;

public static class SpeciesGenerator
{
    public static Species GenerateSpecies()
    {
        return new Species()
        {
            Id = 1,
            Name = "Gato",
            Breeds  = new List<Breed>(),
            Pets = new List<Pet>()
        };
    }

    public static List<Species> GenerateListOfSpecies()
    {
        return new List<Species>()
        {
            GenerateSpecies()
        };
    }
}