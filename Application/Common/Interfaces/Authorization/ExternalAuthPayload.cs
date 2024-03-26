namespace Application.Common.Interfaces.Authorization;

public class ExternalAuthPayload
{
	public required string UserId { get; init; }
	public required string Email { get; init; }
	public required string FullName { get; init; }
	public required string Image { get; init; }
}