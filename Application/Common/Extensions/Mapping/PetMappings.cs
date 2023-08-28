using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class PetMappings
{
    public static PetResponse ConvertToPetResponse(this Pet pet,
        OwnerResponse? owner,
        IEnumerable<ColorResponse> colors,
        BreedResponse breed)
    {
        return new PetResponse()
        {
            Id = pet.Id,
            Name = pet.Name,
            Observations = pet.Observations,
            Owner = owner,
            Breed = breed,
            Colors = colors
        };
    }
}