using System.Collections.Generic;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Tests.EntityGenerators;
using Tests.Fakes.Files;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class FoundAnimalAlertData
	{
		public static readonly Guid Id = Guid.NewGuid();
		public const string Name = "Name";
		public const string Description = "Description";
		public const double FoundLocationLatitude = 32;
		public const double FoundLocationLongitude = 44.322;
		public static readonly DateTime RegistrationDate = new(2020, 1, 1);
		public const bool HasBeenRecovered = false;
		public const string Image = "Image";
		public static readonly IFormFile ImageFile = new EmptyFormFile();
		public static readonly Species Species = SpeciesGenerator.GenerateSpecies();
		public static readonly int SpeciesId = Species.Id;
		public static readonly Breed Breed = BreedGenerator.GenerateBreed();
		public static readonly int BreedId = Breed.Id;
		public static readonly User User = UserGenerator.GenerateUser();
		public static readonly Guid UserId = User.Id;
		public const Gender Gender = Domain.Enums.Gender.Female;
		public static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
		public static readonly List<int> ColorIds = new() { 1 };
	}
}