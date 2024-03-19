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
		public static readonly List<Size>? Sizes = new(1) { Size.Médio };
		public static readonly List<Gender>? Genders = new(1) { Gender.Macho };
		public static readonly List<Age> Ages = new(1) { Age.Jovem };
		public static readonly List<Breed>? Breeds = BreedGenerator.GenerateListOfBreeds();
		public static readonly List<int>? BreedIds = new(1) { 1 };
		public static readonly User User = UserGenerator.GenerateUser();

		public static readonly List<Species>? Species = SpeciesGenerator.GenerateListOfSpecies();
		public static readonly List<int>? SpeciesIds = new() { 1 };

		public static readonly List<Color> Colors = new()
		{
			ColorGenerator.GenerateColor()
		};

		public static readonly List<int> ColorIds = new() { 1 };
	}
}