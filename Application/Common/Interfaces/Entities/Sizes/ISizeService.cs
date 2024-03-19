using Application.Common.Interfaces.FrontendDropdownData;

namespace Application.Common.Interfaces.Entities.Sizes;

public interface ISizeService
{
	IEnumerable<DropdownDataResponse<int>> GetSizes();
}