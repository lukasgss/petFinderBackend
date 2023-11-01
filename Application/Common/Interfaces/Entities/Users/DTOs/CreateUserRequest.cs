using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class CreateUserRequest
{
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}