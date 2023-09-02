using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.DTOs;
using Application.Common.Validations.Alerts.MissingAlertValidations;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
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
    public async Task<ActionResult<MissingAlertResponse>> GetMissingAlertById(Guid alertId)
    {
        MissingAlertResponse missingAlert = await _missingAlertService.GetMissingAlertByIdAsync(alertId);
        return Ok(missingAlert);
    }

    [HttpPost]
    public async Task<ActionResult<MissingAlertResponse>> CreateMissingAlert(
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
            await _missingAlertService.CreateMissingAlertAsync(createAlertRequest, userId);

        return new CreatedAtRouteResult(
            nameof(GetMissingAlertById),
            new { alertId = createdMissingAlert.Id },
            createdMissingAlert);
    }
}