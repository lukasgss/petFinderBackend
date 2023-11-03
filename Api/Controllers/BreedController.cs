using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.FrontendDropdownData;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/breeds")]
public class BreedController : ControllerBase
{
    private readonly IBreedService _breedService;

    public BreedController(IBreedService breedService)
    {
        _breedService = breedService ?? throw new ArgumentNullException(nameof(breedService));
    }

    [HttpGet("dropdown")]
    public async Task<ActionResult<DropdownDataResponse<int>>> GetBreedsByName(string breedName, int speciesId)
    {
        IEnumerable<DropdownDataResponse<int>> breeds = await _breedService.GetBreedsForDropdown(breedName, speciesId);

        return Ok(breeds);
    }
}