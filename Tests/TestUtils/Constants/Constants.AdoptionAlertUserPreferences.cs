using System.Collections.Generic;
using Domain.Entities;
using Domain.Enums;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class AdoptionAlertUserPreferencesData
	{
		public static readonly Guid Id = Guid.NewGuid();
		public static readonly double? Latitude = 35;
		public static readonly double? Longitude = 22;
		public static readonly double? RadiusDistanceInKm = 5;
		public static readonly List<Gender>? Gender = new(1) { Domain.Enums.Gender.Macho };
		public static readonly List<Species>? Species = SpeciesGenerator.GenerateListOfSpecies();
		public static readonly List<Age> Ages = new(1) { Age.Jovem };
		public static readonly List<Breed>? Breed = BreedGenerator.GenerateListOfBreeds();
		public static readonly User User = UserGenerator.GenerateUser();

		public static List<Color> Colors = new()
		{
			ColorGenerator.GenerateColor()
		};

		public static List<int> ColorIds = new() { 1 };
	}
}