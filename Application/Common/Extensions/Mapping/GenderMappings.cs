using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class GenderMappings
{
	public static List<string> ToListOfGenderResponse(this ICollection<Gender>? genders)
	{
		if (genders is null)
		{
			return new List<string>(0);
		}

		return genders.Select(gender => Enum.GetName(typeof(Gender), gender)).ToList()!;
	}
}