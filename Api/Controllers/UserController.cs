using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Validations.Errors;
using Application.Common.Validations.UserValidations;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public UserController(IUserService userService, IUserAuthorizationService userAuthorizationService)
	{
		_userService = userService ?? throw new ArgumentNullException(nameof(userService));
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
	}

	[HttpGet("{userId:guid}", Name = "GetUserById")]
	public async Task<ActionResult<UserDataResponse>> GetUserById(Guid userId)
	{
		return await _userService.GetUserByIdAsync(userId);
	}

	[HttpPost("register")]
	public async Task<ActionResult<UserResponse>> Register(CreateUserRequest createUserRequest)
	{
		RegisterUserValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(createUserRequest);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors
				.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		UserResponse createdUser = await _userService.RegisterAsync(createUserRequest);

		return new CreatedAtRouteResult(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
	}

	[HttpPost("login")]
	public async Task<ActionResult<UserResponse>> Login(LoginUserRequest loginUserRequest)
	{
		LoginUserValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(loginUserRequest);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors
				.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		UserResponse loggedInUser = await _userService.LoginAsync(loginUserRequest);

		return Ok(loggedInUser);
	}

	[HttpPost("external-login")]
	public async Task<ActionResult<UserResponse>> ExternalLogin(ExternalAuthRequest externalAuth)
	{
		return await _userService.ExternalLoginAsync(externalAuth);
	}

	[HttpPost("confirm-email")]
	public async Task<ActionResult> ConfirmEmail(string userId, string token)
	{
		await _userService.ConfirmEmailAsync(userId, token);

		return Ok();
	}

	// This endpoint is meant to be accessed with the refresh token instead of the access token
	[Authorize]
	[HttpPost("refresh")]
	public async Task<ActionResult<TokensResponse>> Refresh()
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _userService.RefreshToken(userId);
	}

	[Authorize]
	[HttpPut("{userRouteId:guid}")]
	public async Task<ActionResult<UserDataResponse>> Update(EditUserRequest editUserRequest, Guid userRouteId)
	{
		EditUserValidator requestValidator = new();
		ValidationResult validationResult = requestValidator.Validate(editUserRequest);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors
				.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
			return BadRequest(errors);
		}

		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _userService.EditAsync(editUserRequest, userId, userRouteId);
	}
}