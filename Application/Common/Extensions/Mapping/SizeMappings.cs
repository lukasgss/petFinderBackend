using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class SizeMappings
{
	public static List<string> ToListOfSizeResponse(this ICollection<Size>? sizes)
	{
		if (sizes is null)
		{
			return new List<string>(0);
		}

		return sizes.Select(size => Enum.GetName(typeof(Size), size)).ToList()!;
	}
}