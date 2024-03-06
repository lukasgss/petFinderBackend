using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Vaccines;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Services.Entities;

public class VaccineService : IVaccineService
{
	private readonly IVaccineRepository _vaccineRepository;

	public VaccineService(IVaccineRepository vaccineRepository)
	{
		_vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
	}

	public async Task<List<DropdownDataResponse<int>>> GetVaccinesOfSpecies(int speciesId)
	{
		List<Vaccine> vaccines = await _vaccineRepository.GetVaccinesOfSpecies(speciesId);

		return vaccines.ToVaccineDropdownResponseList();
	}
}