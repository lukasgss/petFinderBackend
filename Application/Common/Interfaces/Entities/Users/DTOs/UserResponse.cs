namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class UserResponse
{
	public Guid Id { get; set; }
	public string Email { get; set; } = null!;
	public string FullName { get; set; } = null!;
	public string Image { get; set; } = null!;
	public string? PhoneNumber { get; set; }
	public string Token { get; set; } = null!;
}