namespace Application.Common.Interfaces.Authorization.Facebook;

public class FacebookPayload
{
	public required string UserId { get; init; }
	public required string Email { get; init; }
	public required string FullName { get; init; }
	public required string Image { get; init; }
}