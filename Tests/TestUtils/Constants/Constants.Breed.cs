using Domain.Entities;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
    public static class BreedData
    {
        public const int Id = 1;
        public const string Name = "Border Collie";
        public static readonly Species Species = SpeciesGenerator.GenerateSpecies();
        public static readonly int SpeciesId = Species.Id;
    }
}