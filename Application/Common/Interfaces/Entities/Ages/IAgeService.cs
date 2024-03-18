using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Common.Interfaces.Entities.Ages;

public interface IAgeService
{
	Task<List<DropdownDataResponse<int>>> GetAgesAsync();
}