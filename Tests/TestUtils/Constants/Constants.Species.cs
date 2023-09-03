using System.Collections.Generic;
using Domain.Entities;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
    public static class SpeciesData
    {
        public const int Id = 1;
        public const string Name = "Cachorro";
        public static readonly List<Pet> Pets = PetGenerator.GenerateListOfPet();
        public static readonly List<Breed> Breeds = BreedGenerator.GenerateListOfBreeds();
    }
}