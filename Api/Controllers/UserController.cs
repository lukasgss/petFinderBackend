using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserDataResponse>> GetUserById(Guid userId)
    {
        return await _userService.GetUserByIdAsync(userId);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(CreateUserRequest createUserRequest)
    {
        UserResponse createdUser = await _userService.RegisterAsync(createUserRequest);

        return new CreatedAtRouteResult(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginUserRequest loginUserRequest)
    {
        UserResponse loggedInUser = await _userService.LoginAsync(loginUserRequest);

        return Ok(loggedInUser);
    }
}