using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Validations.Errors;
using Application.Common.Validations.UserValidations;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserDataResponse>> GetUserById(Guid userId)
    {
        return await _userService.GetUserByIdAsync(userId);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromForm] CreateUserRequest createUserRequest)
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

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(string userId, string token)
    {
        await _userService.ConfirmEmailAsync(userId, token);

        return Ok();
    }
}