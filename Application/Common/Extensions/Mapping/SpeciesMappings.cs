using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class SpeciesMappings
{
	public static SpeciesResponse ToSpeciesResponse(this Species species)
	{
		return new SpeciesResponse()
		{
			Id = species.Id,
			Name = species.Name
		};
	}

	public static List<SpeciesResponse> ToListOfSpeciesResponse(this ICollection<Species>? species)
	{
		if (species is null)
		{
			return new List<SpeciesResponse>(0);
		}

		return species.Select(s => new SpeciesResponse()
		{
			Id = s.Id,
			Name = s.Name
		}).ToList();
	}

	public static DropdownDataResponse<int> ToDropdownData(this Species species)
	{
		return new DropdownDataResponse<int>()
		{
			Text = species.Name,
			Value = species.Id
		};
	}

	public static List<DropdownDataResponse<int>> ToListOfDropdownData(this IEnumerable<Species> species)
	{
		List<DropdownDataResponse<int>> dropdownDataSpecies = new();
		foreach (Species speciesValue in species)
		{
			dropdownDataSpecies.Add(speciesValue.ToDropdownData());
		}

		return dropdownDataSpecies;
	}
}