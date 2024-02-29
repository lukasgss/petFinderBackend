using Application.Common.Interfaces.Entities.Vaccines.DTOs;

namespace Application.Common.Interfaces.Entities.Vaccines;

public interface IVaccineService
{
	Task<List<VaccineResponse>> GetVaccinesOfSpecies(int speciesId);
}