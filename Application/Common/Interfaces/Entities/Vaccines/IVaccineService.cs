using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Common.Interfaces.Entities.Vaccines;

public interface IVaccineService
{
	Task<List<DropdownDataResponse<int>>> GetVaccinesOfSpecies(int speciesId);
}