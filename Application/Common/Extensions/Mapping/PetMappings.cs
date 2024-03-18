using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

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
			Age = Enum.GetName(typeof(Age), pet.Age)!,
			Size = Enum.GetName(typeof(Size), pet.Size)!,
			Images = pet.Images.ToPetImagesResponse(),
			Gender = Enum.GetName(typeof(Gender), pet.Gender)!,
			Owner = pet.Owner.ToOwnerResponse(),
			Breed = pet.Breed.ToBreedResponse(),
			Colors = pet.Colors.ToListOfColorResponse(),
			Vaccines = pet.Vaccines.ToVaccineResponseList()
		};
	}

	public static PetResponseNoOwner ToPetResponseNoOwner(this Pet pet)
	{
		return new PetResponseNoOwner()
		{
			Id = pet.Id,
			Name = pet.Name,
			Observations = pet.Observations,
			Age = Enum.GetName(typeof(Age), pet.Age)!,
			Size = Enum.GetName(typeof(Size), pet.Size)!,
			Images = pet.Images.ToPetImagesResponse(),
			Gender = Enum.GetName(typeof(Gender), pet.Gender)!,
			Breed = pet.Breed.ToBreedResponse(),
			Colors = pet.Colors.ToListOfColorResponse(),
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
			Age = Enum.GetName(typeof(Age), pet.Age)!,
			Size = Enum.GetName(typeof(Size), pet.Size)!,
			Images = pet.Images.ToPetImagesResponse(),
			Gender = Enum.GetName(typeof(Gender), pet.Gender)!,
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