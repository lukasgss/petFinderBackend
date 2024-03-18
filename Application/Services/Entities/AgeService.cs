using Application.Common.Interfaces.Entities.Ages;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Enums;

namespace Application.Services.Entities;

public class AgeService : IAgeService
{
	private readonly List<DropdownDataResponse<int>> _ages = new();

	public AgeService()
	{
		foreach (Age age in Enum.GetValues<Age>())
		{
			_ages.Add(new DropdownDataResponse<int>()
			{
				Text = Enum.GetName(typeof(Age), age)!,
				Value = (int)age
			});
		}
	}

	public List<DropdownDataResponse<int>> GetAges()
	{
		return _ages;
	}
}