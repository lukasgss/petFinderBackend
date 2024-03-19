using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
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
		public const double FoundLocationLongitude = 44;
		public static readonly DateTime RegistrationDate = new(2020, 1, 1);
		public static readonly DateOnly? RecoveryDate = null;
		public static readonly List<string> ImageUrls = new(1) { "Image" };
		public static readonly List<IFormFile> ImageFiles = new(1) { new EmptyFormFile() };

		public static readonly List<FoundAnimalAlertImage> FoundAnimalAlertImages = new()
		{
			new FoundAnimalAlertImage() { Id = 1, ImageUrl = ImageUrls.First(), FoundAnimalAlertId = Id }
		};

		public static readonly Species Species = SpeciesGenerator.GenerateSpecies();
		public static readonly int SpeciesId = Species.Id;
		public static readonly Breed Breed = BreedGenerator.GenerateBreed();
		public static readonly int BreedId = Breed.Id;
		public static readonly Age Age = Age.Jovem;
		public static readonly Size Size = Size.Grande;
		public static readonly User User = UserGenerator.GenerateUser();
		public static readonly Guid UserId = User.Id;
		public const Gender Gender = Domain.Enums.Gender.Fêmea;
		public static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
		public static readonly List<int> ColorIds = new() { 1 };
	}
}