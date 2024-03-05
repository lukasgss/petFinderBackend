using Application.ApplicationConstants;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Validations.Alerts.FoundAnimalAlertValidations;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/found-alerts")]
public class FoundAnimalAlertController : ControllerBase
{
	private readonly IFoundAnimalAlertService _foundAnimalAlertService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public FoundAnimalAlertController(
		IFoundAnimalAlertService foundAnimalAlertService,
		IUserAuthorizationService userAuthorizationService)
	{
		_foundAnimalAlertService =
			foundAnimalAlertService ?? throw new ArgumentNullException(nameof(foundAnimalAlertService));
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
	}

	[HttpGet("{alertId:guid}", Name = "GetFoundAlertById")]
	public async Task<ActionResult<FoundAnimalAlertResponse>> GetFoundAlertById(Guid alertId)
	{
		return await _foundAnimalAlertService.GetByIdAsync(alertId);
	}

	[HttpGet]
	public async Task<ActionResult<PaginatedEntity<FoundAnimalAlertResponse>>> ListFoundAnimalAlerts(
		[FromQuery] FoundAnimalAlertFilters filters,
		int page = 1,
		int pageSize = Constants.DefaultPageSize)
	{
		return await _foundAnimalAlertService.ListFoundAnimalAlerts(filters, page, pageSize);
	}

	[HttpPost]
	public async Task<ActionResult<FoundAnimalAlertResponse>> Create(
		[FromForm] CreateFoundAnimalAlertRequest createRequest)
	{
		CreateFoundAnimalAlertValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(createRequest);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors
				.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		FoundAnimalAlertResponse createdAlert = await _foundAnimalAlertService.CreateAsync(createRequest, userId);

		return new CreatedAtRouteResult(nameof(GetFoundAlertById), new { alertId = createdAlert.Id }, createdAlert);
	}

	[HttpPut("{alertId:guid}")]
	public async Task<ActionResult<FoundAnimalAlertResponse>> Edit(
		[FromForm] EditFoundAnimalAlertRequest editRequest, Guid alertId)
	{
		EditFoundAnimalAlertValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(editRequest);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors
				.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _foundAnimalAlertService.EditAsync(editRequest, userId, alertId);
	}

	[HttpPut("rescue/{alertId:guid}")]
	public async Task<ActionResult<FoundAnimalAlertResponse>> ToggleStatus(Guid alertId)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _foundAnimalAlertService.ToggleAlertStatus(alertId, userId);
	}

	[HttpDelete("{alertId:guid}")]
	public async Task<ActionResult> Delete(Guid alertId)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		await _foundAnimalAlertService.DeleteAsync(alertId, userId);
		return NoContent();
	}
}