namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class LoginUserRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}