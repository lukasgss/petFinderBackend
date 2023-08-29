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

    public PetController(IPetService petService)
    {
        _petService = petService;
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
        
        PetResponse createdPet = await _petService.CreatePetAsync(createPetRequest);

        return new CreatedAtRouteResult(nameof(GetPetById), new { petId = createdPet.Id }, createdPet);
    }
}