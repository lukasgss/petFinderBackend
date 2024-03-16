namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class UserResponse
{
	public required Guid Id { get; init; }
	public required string Email { get; init; }
	public required string FullName { get; init; }
	public required string Image { get; init; }
	public string? PhoneNumber { get; init; }
	public required string AccessToken { get; init; }
	public required string RefreshToken { get; init; }
}