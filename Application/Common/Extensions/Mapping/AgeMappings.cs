using Application.Common.Interfaces.Entities.AnimalSpecies;
using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class AgeMappings
{
	public static List<string> ToListOfAgeResponse(this ICollection<Age>? ages)
	{
		if (ages is null)
		{
			return new List<string>(0);
		}

		return ages.Select(age => Enum.GetName(typeof(Age), age)).ToList()!;
	}
}