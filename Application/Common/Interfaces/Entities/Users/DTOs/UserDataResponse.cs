namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class UserDataResponse
{
	public Guid Id { get; set; }
	public string Email { get; set; } = null!;
	public string Image { get; set; } = null!;
	public string FullName { get; set; } = null!;
	public string? PhoneNumber { get; set; }
}