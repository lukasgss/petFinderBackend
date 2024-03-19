using Application.Common.Interfaces.Entities.Sizes;
using Application.Common.Interfaces.FrontendDropdownData;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/sizes")]
public class SizeController : ControllerBase
{
	private readonly ISizeService _sizeService;

	public SizeController(ISizeService sizeService)
	{
		_sizeService = sizeService ?? throw new ArgumentNullException(nameof(sizeService));
	}

	[HttpGet]
	public ActionResult<IEnumerable<DropdownDataResponse<int>>> ListSizes()
	{
		var sizes = _sizeService.GetSizes();
		return Ok(sizes);
	}
}