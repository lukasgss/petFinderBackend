namespace Application.Common.Interfaces.Authorization.Google;

public class GoogleAuthConfig
{
	public required string ClientId { get; set; } = null!;
	public required string ClientSecret { get; set; } = null!;
}