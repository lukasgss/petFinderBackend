using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Validations.Alerts.UserPreferences;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.UserPreferences;

[ApiController]
[Route("/api/user-preferences/adoptions")]
public class AdoptionUserPreferencesController : ControllerBase
{
	private readonly IAdoptionAlertUserPreferencesService _adoptionAlertUserPreferencesService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public AdoptionUserPreferencesController(IAdoptionAlertUserPreferencesService adoptionAlertUserPreferencesService,
		IUserAuthorizationService userAuthorizationService)
	{
		_adoptionAlertUserPreferencesService = adoptionAlertUserPreferencesService ??
		                                       throw new ArgumentNullException(
			                                       nameof(adoptionAlertUserPreferencesService));
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
	}

	[Authorize]
	[HttpGet(Name = "GetAdoptionUserPreferences")]
	public async Task<ActionResult<UserPreferencesResponse>> GetAdoptionUserPreferences()
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _adoptionAlertUserPreferencesService.GetUserPreferences(userId);
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

		UserPreferencesResponse userPreferences =
			await _adoptionAlertUserPreferencesService.CreatePreferences(createUserPreferences, userId);

		return new CreatedAtRouteResult(nameof(GetAdoptionUserPreferences), null, userPreferences);
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

		return await _adoptionAlertUserPreferencesService.EditPreferences(editUserPreferences, userId);
	}
}