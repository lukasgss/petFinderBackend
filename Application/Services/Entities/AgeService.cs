using Application.Common.Interfaces.Entities.Ages;
using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Services.Entities;

public class AgeService : IAgeService
{
	private readonly IAgeRepository _ageRepository;

	public AgeService(IAgeRepository ageRepository)
	{
		_ageRepository = ageRepository ?? throw new ArgumentNullException(nameof(ageRepository));
	}

	public async Task<List<DropdownDataResponse<int>>> GetAgesAsync()
	{
		var ages = await _ageRepository.GetAll();

		return ages.Select(age => new DropdownDataResponse<int>()
			{
				Text = age.Name,
				Value = age.Id
			})
			.ToList();
	}
}