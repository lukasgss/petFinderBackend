using Application.Common.Interfaces.Entities.Ages;
using Application.Common.Interfaces.FrontendDropdownData;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/ages")]
public class AgeController : ControllerBase
{
	private readonly IAgeService _ageService;

	public AgeController(IAgeService ageService)
	{
		_ageService = ageService ?? throw new ArgumentNullException(nameof(ageService));
	}

	[HttpGet]
	public async Task<ActionResult<List<DropdownDataResponse<int>>>> ListAges()
	{
		return await _ageService.GetAgesAsync();
	}
}