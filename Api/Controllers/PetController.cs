using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Validations.Errors;
using Application.Common.Validations.PetValidations;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("/pets")]
[ApiController]
public class PetController : ControllerBase
{
    private readonly IPetService _petService;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public PetController(IPetService petService, IUserAuthorizationService userAuthorizationService)
    {
        _petService = petService ?? throw new ArgumentNullException(nameof(petService));
        _userAuthorizationService = userAuthorizationService ??
                                    throw new ArgumentNullException(nameof(userAuthorizationService));
    }

    [HttpGet("{petId:guid}", Name = "GetPetById")]
    public async Task<ActionResult<PetResponse>> GetPetById(Guid petId)
    {
        return await _petService.GetPetBydIdAsync(petId);
    }

    [HttpPost]
    public async Task<ActionResult<PetResponse>> CreatePet(CreatePetRequest createPetRequest)
    {
        CreatePetValidator requestValidator = new();
        ValidationResult validationResult = requestValidator.Validate(createPetRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        Guid? userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        PetResponse createdPet = await _petService.CreatePetAsync(createPetRequest, userId);

        return new CreatedAtRouteResult(nameof(GetPetById), new { petId = createdPet.Id }, createdPet);
    }
}