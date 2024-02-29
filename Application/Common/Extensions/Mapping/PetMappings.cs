using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;
using Color = Domain.Entities.Color;

namespace Application.Common.Extensions.Mapping;

public static class PetMappings
{
	public static PetResponse ToPetResponse(this Pet pet,
		User? owner,
		IEnumerable<Color> colors,
		Breed breed)
	{
		return new PetResponse()
		{
			Id = pet.Id,
			Name = pet.Name,
			Observations = pet.Observations,
			AgeInMonths = pet.AgeInMonths,
			Image = pet.Image,
			Gender = pet.Gender.ToString(),
			Owner = owner?.ToOwnerResponse(),
			Breed = breed.ToBreedResponse(),
			Colors = colors.ToListOfColorResponse(),
			Vaccines = pet.Vaccines.ToVaccineResponseList()
		};
	}

	public static PetResponseNoOwner ToPetResponseNoOwner(this Pet pet,
		IEnumerable<Color> colors,
		Breed breed)
	{
		return new PetResponseNoOwner()
		{
			Id = pet.Id,
			Name = pet.Name,
			Observations = pet.Observations,
			AgeInMonths = pet.AgeInMonths,
			Image = pet.Image,
			Gender = pet.Gender.ToString(),
			Breed = breed.ToBreedResponse(),
			Colors = colors.ToListOfColorResponse(),
			Vaccines = pet.Vaccines.ToVaccineResponseList()
		};
	}
}