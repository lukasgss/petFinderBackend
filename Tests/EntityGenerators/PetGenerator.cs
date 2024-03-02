using System.Collections.Generic;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.EntityGenerators;

public static class PetGenerator
{
	public static Pet GeneratePet()
	{
		return new Pet()
		{
			Id = Constants.PetData.Id,
			Name = Constants.PetData.Name,
			Observations = Constants.PetData.Observations,
			Gender = Constants.PetData.Gender,
			AgeInMonths = Constants.PetData.AgeInMonths,
			Images = Constants.PetData.PetImages,
			Owner = Constants.PetData.User,
			UserId = Constants.UserData.Id,
			Breed = BreedGenerator.GenerateBreed(),
			BreedId = Constants.BreedData.Id,
			Species = SpeciesGenerator.GenerateSpecies(),
			SpeciesId = Constants.SpeciesData.Id,
			Colors = ColorGenerator.GenerateListOfColors(),
			Vaccines = new List<Vaccine>(0)
		};
	}

	public static Pet GeneratePetWithRandomOwnerId()
	{
		return new Pet()
		{
			Id = Constants.PetData.Id,
			Name = Constants.PetData.Name,
			Observations = Constants.PetData.Observations,
			Gender = Constants.PetData.Gender,
			AgeInMonths = Constants.PetData.AgeInMonths,
			Images = Constants.PetData.PetImages,
			Owner = UserGenerator.GenerateUserWithRandomId(),
			UserId = UserGenerator.GenerateUserWithRandomId().Id,
			Breed = BreedGenerator.GenerateBreed(),
			BreedId = Constants.BreedData.Id,
			Species = SpeciesGenerator.GenerateSpecies(),
			SpeciesId = Constants.SpeciesData.Id,
			Colors = ColorGenerator.GenerateListOfColors(),
			Vaccines = new List<Vaccine>(0)
		};
	}

	public static List<Pet> GenerateListOfPet()
	{
		return new List<Pet>()
		{
			GeneratePet()
		};
	}

	public static CreatePetRequest GenerateCreatePetRequest()
	{
		return new CreatePetRequest()
		{
			Name = Constants.PetData.Name,
			Observations = Constants.PetData.Observations,
			Gender = Constants.PetData.Gender,
			AgeInMonths = Constants.PetData.AgeInMonths,
			Images = Constants.PetData.ImageFiles,
			BreedId = Constants.PetData.BreedId,
			SpeciesId = Constants.PetData.SpeciesId,
			ColorIds = Constants.PetData.ColorIds
		};
	}

	public static EditPetRequest GenerateEditPetRequest()
	{
		return new EditPetRequest()
		{
			Id = Constants.PetData.Id,
			Name = Constants.PetData.Name,
			Observations = Constants.PetData.Observations,
			Gender = Constants.PetData.Gender,
			AgeInMonths = Constants.PetData.AgeInMonths,
			Images = Constants.PetData.ImageFiles,
			BreedId = Constants.PetData.BreedId,
			SpeciesId = Constants.PetData.SpeciesId,
			ColorIds = Constants.PetData.ColorIds
		};
	}
}