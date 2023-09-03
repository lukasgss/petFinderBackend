using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.FrontendDropdownData;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/species")]
public class SpeciesController : ControllerBase
{
    private readonly ISpeciesService _speciesService;

    public SpeciesController(ISpeciesService speciesService)
    {
        _speciesService = speciesService ?? throw new ArgumentNullException(nameof(speciesService));
    }

    [HttpGet("dropdown")]
    public async Task<ActionResult<DropdownDataResponse<int>>> GetAllSpecies()
    {
        var species = await _speciesService.GetAllSpeciesForDropdown();
        return Ok(species);
    }
}