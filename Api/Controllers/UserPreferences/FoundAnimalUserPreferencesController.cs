using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.FoundAnimalAlerts;
using Application.Common.Validations.Alerts.UserPreferences;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.UserPreferences;

[ApiController]
[Route("/api/user-preferences/found-animals")]
public class FoundAnimalUserPreferencesController : ControllerBase
{
	private readonly IFoundAnimalUserPreferencesService _foundAnimalUserPreferencesService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public FoundAnimalUserPreferencesController(IFoundAnimalUserPreferencesService foundAnimalUserPreferencesService,
		IUserAuthorizationService userAuthorizationService)
	{
		_foundAnimalUserPreferencesService = foundAnimalUserPreferencesService ??
		                                     throw new ArgumentNullException(nameof(foundAnimalUserPreferencesService));
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
	}

	[Authorize]
	[HttpGet(Name = "GetFoundAnimalUserPreferences")]
	public async Task<ActionResult<UserPreferencesResponse>> GetFoundAnimalUserPreferences()
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _foundAnimalUserPreferencesService.GetUserPreferences(userId);
	}

	[Authorize]
	[HttpPost]
	public async Task<ActionResult<UserPreferencesResponse>> CreatePreferences(
		CreateAlertsUserPreferences createUserPreferences)
	{
		CreateAlertsUserPreferencesValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(createUserPreferences);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors
				.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		var userPreferences = await _foundAnimalUserPreferencesService.CreatePreferences(createUserPreferences, userId);

		return new CreatedAtRouteResult(nameof(GetFoundAnimalUserPreferences), null, userPreferences);
	}

	[Authorize]
	[HttpPut]
	public async Task<ActionResult<UserPreferencesResponse>> CreatePreferences(
		EditAlertsUserPreferences editUserPreferences)
	{
		EditAlertsUserPreferencesValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(editUserPreferences);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors
				.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _foundAnimalUserPreferencesService.EditPreferences(editUserPreferences, userId);
	}
}