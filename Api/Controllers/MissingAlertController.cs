using Application.ApplicationConstants;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Validations.Alerts.MissingAlertValidations;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/missing-alerts")]
public class MissingAlertController : ControllerBase
{
    private readonly IMissingAlertService _missingAlertService;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public MissingAlertController(IMissingAlertService missingAlertService,
        IUserAuthorizationService userAuthorizationService)
    {
        _missingAlertService = missingAlertService ?? throw new ArgumentNullException(nameof(missingAlertService));
        _userAuthorizationService = userAuthorizationService ??
                                    throw new ArgumentNullException(nameof(userAuthorizationService));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedEntity<MissingAlertResponse>>> ListMissingAlerts(
        [FromQuery] MissingAlertFilters missingAlertFilters,
        int page = 1,
        int pageSize = Constants.DefaultPageSize)
    {
        return await _missingAlertService.ListMissingAlerts(missingAlertFilters, page, pageSize);
    }

    [HttpGet("{alertId:guid}", Name = "GetMissingAlertById")]
    public async Task<ActionResult<MissingAlertResponse>> GetMissingAlertById(Guid alertId)
    {
        MissingAlertResponse missingAlert = await _missingAlertService.GetByIdAsync(alertId);
        return Ok(missingAlert);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<MissingAlertResponse>> Create(
        CreateMissingAlertRequest createAlertRequest)
    {
        CreateMissingAlertValidator requestValidator = new();
        ValidationResult validationResult = requestValidator.Validate(createAlertRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        MissingAlertResponse createdMissingAlert =
            await _missingAlertService.CreateAsync(createAlertRequest, userId);

        return new CreatedAtRouteResult(
            nameof(GetMissingAlertById),
            new { alertId = createdMissingAlert.Id },
            createdMissingAlert);
    }

    [Authorize]
    [HttpPut("{alertId:guid}")]
    public async Task<ActionResult<MissingAlertResponse>> Edit(EditMissingAlertRequest editMissingAlertRequest,
        Guid alertId)
    {
        EditMissingAlertValidator requestValidator = new();
        ValidationResult validationResult = requestValidator.Validate(editMissingAlertRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        MissingAlertResponse editedAlert =
            await _missingAlertService.EditAsync(editMissingAlertRequest, userId, alertId);

        return Ok(editedAlert);
    }

    [Authorize]
    [HttpPost("find/{alertId:guid}")]
    public async Task<ActionResult<MissingAlertResponse>> MarkAsFound(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);
        MissingAlertResponse missingAlertResponse = await _missingAlertService.ToggleFoundStatusAsync(alertId, userId);

        return Ok(missingAlertResponse);
    }

    [Authorize]
    [HttpDelete("{alertId:guid}")]
    public async Task<ActionResult> Delete(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);
        await _missingAlertService.DeleteAsync(alertId, userId);

        return NoContent();
    }
}