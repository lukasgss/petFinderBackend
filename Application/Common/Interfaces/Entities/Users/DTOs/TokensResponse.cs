namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class TokensResponse
{
	public required string AccessToken { get; init; }
	public required string RefreshToken { get; init; }
}