using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Common.Interfaces.Entities.Ages;

public interface IAgeService
{
	List<DropdownDataResponse<int>> GetAges();
}