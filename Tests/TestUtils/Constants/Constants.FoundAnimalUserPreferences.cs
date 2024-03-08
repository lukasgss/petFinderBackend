using System.Collections.Generic;
using Domain.Entities;
using Domain.Enums;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class FoundAnimalUserPreferencesData
	{
		public static readonly Guid Id = Guid.NewGuid();
		public static readonly double? FoundLocationLatitude = 35;
		public static readonly double? FoundLocationLongitude = 22;
		public static readonly double? RadiusDistanceInKm = 5;
		public static readonly Gender? Gender = Domain.Enums.Gender.Female;
		public static readonly Species? Species = SpeciesGenerator.GenerateSpecies();
		public static readonly int SpeciesId = Species.Id;
		public static readonly Breed? Breed = BreedGenerator.GenerateBreed();
		public static readonly int? BreedId = Breed.Id;
		public static readonly User User = UserGenerator.GenerateUser();
		public static readonly Guid UserId = User.Id;

		public static List<Color> Colors = new()
		{
			ColorGenerator.GenerateColor()
		};

		public static List<int> ColorIds = new() { 1 };
	}
}