using Application.Common.Interfaces.Entities.Genders;
using Application.Common.Interfaces.FrontendDropdownData;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/genders")]
public class GenderController : ControllerBase
{
	private readonly IGenderService _genderService;

	public GenderController(IGenderService genderService)
	{
		_genderService = genderService ?? throw new ArgumentNullException(nameof(genderService));
	}

	[HttpGet]
	public ActionResult<List<DropdownDataResponse<int>>> ListAges()
	{
		var genders = _genderService.GetGenders();
		return Ok(genders);
	}
}