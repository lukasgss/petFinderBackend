using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Genders;

public interface IGenderService
{
	IEnumerable<DropdownDataResponse<int>> GetGenders();
}