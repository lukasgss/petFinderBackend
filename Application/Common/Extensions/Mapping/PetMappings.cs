using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;

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
            Owner = owner?.ToOwnerResponse(),
            Breed = breed.ToBreedResponse(),
            Colors = colors.ToListOfColorResponse()
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
            Breed = breed.ToBreedResponse(),
            Colors = colors.ToListOfColorResponse()
        };
    }
}