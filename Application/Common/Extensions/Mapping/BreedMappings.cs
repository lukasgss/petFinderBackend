using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class BreedMappings
{
    public static BreedResponse ToBreedResponse(this Breed breed)
    {
        return new BreedResponse()
        {
            Id = breed.Id,
            Name = breed.Name
        };
    }

    public static DropdownDataResponse<int> ToDropdownData(this Breed breed)
    {
        return new DropdownDataResponse<int>()
        {
            Text = breed.Name,
            Value = breed.Id
        };
    }

    public static List<DropdownDataResponse<int>> ToListOfDropdownData(this IEnumerable<Breed> breeds)
    {
        List<DropdownDataResponse<int>> dropdownDataBreeds= new();
        foreach (Breed breed in breeds)
        {
            dropdownDataBreeds.Add(breed.ToDropdownData());
        }

        return dropdownDataBreeds;
    }
}