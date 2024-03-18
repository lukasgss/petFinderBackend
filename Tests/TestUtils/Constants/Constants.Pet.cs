using System.Collections.Generic;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Tests.EntityGenerators;
using Tests.Fakes.Files;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class PetData
	{
		public static readonly Guid Id = Guid.NewGuid();
		public const string Name = "Pet Name";
		public const string? Observations = "Observations";
		public static readonly Gender Gender = Gender.Macho;
		public static readonly Size Size = Size.Médio;
		public static readonly Age Age = Age.Jovem;
		public static readonly List<PetImage> PetImages = new(0);
		public static readonly List<string> ImageUrls = new() { ImageUrl };
		public const string ImageUrl = "ImageUrl";
		public static readonly List<IFormFile> ImageFiles = new() { new EmptyFormFile() };
		public static readonly User User = UserGenerator.GenerateUser();
		public static readonly Guid? UserId = UserData.Id;
		public static readonly Breed Breed = BreedGenerator.GenerateBreed();
		public static readonly int BreedId = Breed.Id;
		public static readonly Species Species = SpeciesGenerator.GenerateSpecies();
		public static readonly int SpeciesId = Species.Id;
		public static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
		public static readonly List<int> ColorIds = new() { 1 };
		public static readonly List<int> VaccineIds = new() { 1 };
	}
}