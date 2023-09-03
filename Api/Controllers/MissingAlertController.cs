using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.DTOs;
using Application.Common.Validations.Alerts.MissingAlertValidations;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/missing-alerts")]
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

    [HttpGet("{alertId:guid}", Name = "GetMissingAlertById")]
    public async Task<ActionResult<MissingAlertResponse>> GetById(Guid alertId)
    {
        MissingAlertResponse missingAlert = await _missingAlertService.GetByIdAsync(alertId);
        return Ok(missingAlert);
    }

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

        Guid? userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        MissingAlertResponse createdMissingAlert =
            await _missingAlertService.CreateAsync(createAlertRequest, userId);

        return new CreatedAtRouteResult(
            nameof(GetById),
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

        Guid? userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        MissingAlertResponse editedAlert =
            await _missingAlertService.EditAsync(editMissingAlertRequest, userId, alertId);

        return Ok(editedAlert);
    }

    [Authorize]
    [HttpDelete("{alertId:guid}")]
    public async Task<ActionResult> Delete(Guid alertId)
    {
        Guid? userId = _userAuthorizationService.GetUserIdFromJwtToken(User);
        await _missingAlertService.DeleteAsync(alertId, userId);
        
        return Ok();
    }
}