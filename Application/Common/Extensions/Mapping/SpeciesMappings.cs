using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class SpeciesMappings
{
    public static DropdownDataResponse<int> ConvertToDropdownData(this Species species)
    {
        return new DropdownDataResponse<int>()
        {
            Text = species.Name,
            Value = species.Id
        };
    }
    
    public static List<DropdownDataResponse<int>> ConvertToListOfDropdownData(this IEnumerable<Species> species)
    {
        List<DropdownDataResponse<int>> dropdownDataSpecies = new();
        foreach (Species speciesValue in species)
        {
            dropdownDataSpecies.Add(speciesValue.ConvertToDropdownData());
        }

        return dropdownDataSpecies;
    }
}