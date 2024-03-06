using Application.Common.Interfaces.Entities.Vaccines;
using Application.Common.Interfaces.FrontendDropdownData;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/vaccines")]
public class VaccineController : ControllerBase
{
	private readonly IVaccineService _vaccineService;

	public VaccineController(IVaccineService vaccineService)
	{
		_vaccineService = vaccineService ?? throw new ArgumentNullException(nameof(vaccineService));
	}

	[HttpGet]
	public async Task<ActionResult<List<DropdownDataResponse<int>>>> GetVaccines(int speciesId)
	{
		return await _vaccineService.GetVaccinesOfSpecies(speciesId);
	}
}