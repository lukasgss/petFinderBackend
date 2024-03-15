using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;
using Domain.ValueObjects;
using Color = Domain.Entities.Color;

namespace Application.Common.Extensions.Mapping;

public static class PetMappings
{
	public static PetResponse ToPetResponse(this Pet pet)
	{
		return new PetResponse()
		{
			Id = pet.Id,
			Name = pet.Name,
			Observations = pet.Observations,
			AgeInMonths = pet.AgeInMonths,
			Images = pet.Images.ToPetImagesResponse(),
			Gender = pet.Gender.ToString(),
			Owner = pet.Owner.ToOwnerResponse(),
			Breed = pet.Breed.ToBreedResponse(),
			Colors = pet.Colors.ToListOfColorResponse(),
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
			Images = pet.Images.ToPetImagesResponse(),
			Gender = pet.Gender.ToString(),
			Breed = breed.ToBreedResponse(),
			Colors = colors.ToListOfColorResponse(),
			Vaccines = pet.Vaccines.ToVaccineResponseList()
		};
	}

	public static SimplifiedPetResponse ToSimplifiedPetResponse(this Pet pet)
	{
		return new SimplifiedPetResponse()
		{
			Id = pet.Id,
			Name = pet.Name,
			Observations = pet.Observations,
			AgeInMonths = pet.AgeInMonths,
			Images = pet.Images.ToPetImagesResponse(),
			Gender = pet.Gender.ToString(),
		};
	}

	private static List<string> ToPetImagesResponse(this IEnumerable<PetImage> images)
	{
		List<string> imageUrls = new();
		foreach (PetImage petImage in images)
		{
			imageUrls.Add(petImage.ImageUrl);
		}

		return imageUrls;
	}
}