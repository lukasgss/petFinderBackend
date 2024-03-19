using Application.Common.Interfaces.Entities.Genders;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Enums;

namespace Application.Services.Entities;

public class GenderService : IGenderService
{
	private readonly List<DropdownDataResponse<int>> _genders = new();

	public GenderService()
	{
		foreach (Gender gender in Enum.GetValues<Gender>())
		{
			_genders.Add(new DropdownDataResponse<int>()
			{
				Text = Enum.GetName(typeof(Gender), gender)!,
				Value = (int)gender
			});
		}
	}

	public IEnumerable<DropdownDataResponse<int>> GetGenders()
	{
		return _genders;
	}
}