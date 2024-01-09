using Application.ApplicationConstants;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Validations.Alerts.AdoptionAlertValidations;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/adoption-alerts")]
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

    [HttpGet]
    public async Task<ActionResult<PaginatedEntity<AdoptionAlertResponse>>> ListAdoptionAlerts(
        [FromQuery] AdoptionAlertFilters adoptionAlertFilters,
        int page = 1,
        int pageSize = Constants.DefaultPageSize)
    {
        return await _adoptionAlertService.ListAdoptionAlerts(adoptionAlertFilters, page, pageSize);
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

    [Authorize]
    [HttpPost("adopt/{alertId:guid}")]
    public async Task<ActionResult<AdoptionAlertResponse>> MarkAsAdopted(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _adoptionAlertService.ToggleAdoptionAsync(alertId, userId);
    }

    [Authorize]
    [HttpPut("{alertId:guid}")]
    public async Task<ActionResult<AdoptionAlertResponse>> Edit(EditAdoptionAlertRequest editAlertRequest, Guid alertId)
    {
        EditAdoptionAlertValidator requestValidator = new();
        ValidationResult validationResult = requestValidator.Validate(editAlertRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _adoptionAlertService.EditAsync(editAlertRequest, userId, alertId);
    }

    [Authorize]
    [HttpDelete("{alertId:guid}")]
    public async Task<ActionResult> Delete(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        await _adoptionAlertService.DeleteAsync(alertId, userId);
        return NoContent();
    }
}