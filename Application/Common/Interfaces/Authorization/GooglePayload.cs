namespace Application.Common.Interfaces.Authorization;

public class GooglePayload
{
	public required string Email { get; init; }
	public required string Subject { get; init; }
	public required string FullName { get; init; }
	public required string Image { get; set; }
}