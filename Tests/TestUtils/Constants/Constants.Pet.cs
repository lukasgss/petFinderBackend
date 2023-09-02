using System.Collections.Generic;
using Domain.Entities;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
    public static class PetData
    {
        public static readonly Guid Id = Guid.NewGuid();
        public const string Name = "Pet Name";
        public const string? Observations = "Observations";
        public static readonly User User = UserGenerator.GenerateUser();
        public static readonly Guid? UserId = UserData.Id;
        public static readonly Breed Breed = BreedGenerator.GenerateBreed();
        public static readonly int BreedId = Breed.Id;
        public static readonly Species Species = SpeciesGenerator.GenerateSpecies();
        public static readonly int SpeciesId = Species.Id;
        public static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
        public static readonly List<int> ColorIds = new() { 1 };
    }
}