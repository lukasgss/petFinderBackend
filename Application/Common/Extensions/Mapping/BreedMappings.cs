using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class BreedMappings
{
    public static BreedResponse ConvertToBreedResponse(this Breed breed)
    {
        return new BreedResponse()
        {
            Id = breed.Id,
            Name = breed.Name
        };
    }
}