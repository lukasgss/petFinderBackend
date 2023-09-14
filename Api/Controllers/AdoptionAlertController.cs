using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Validations.Alerts.AdoptionAlertValidations;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/adoption-alerts")]
public class AdoptionAlertController : ControllerBase
{
    private readonly IAdoptionAlertService _adoptionAlertService;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public AdoptionAlertController(
        IAdoptionAlertService adoptionAlertService,
        IUserAuthorizationService userAuthorizationService)
    {
        _adoptionAlertService = adoptionAlertService ?? throw new ArgumentNullException(nameof(adoptionAlertService));
        _userAuthorizationService = userAuthorizationService ??
                                    throw new ArgumentNullException(nameof(userAuthorizationService));
    }

    [HttpGet("{alertId:guid}", Name = "GetAdoptionAlertById")]
    public async Task<ActionResult<AdoptionAlertResponse>> GetAdoptionAlertById(Guid alertId)
    {
        return await _adoptionAlertService.GetByIdAsync(alertId);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AdoptionAlertResponse>> Create(CreateAdoptionAlertRequest createAlertRequest)
    {
        CreateAdoptionAlertValidator requestValidator = new();
        ValidationResult validationResult = requestValidator.Validate(createAlertRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        var adoptionAlertResponse = await _adoptionAlertService.CreateAsync(createAlertRequest, userId);
        return new CreatedAtRouteResult(
            nameof(GetAdoptionAlertById),
            new { alertId = adoptionAlertResponse.Id },
            adoptionAlertResponse);
    }
}