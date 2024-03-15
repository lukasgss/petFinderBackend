namespace Application.Common.Interfaces.Authorization;

public class ExternalAuthPayload
{
	public required string Email { get; set; }
	public required string FullName { get; set; }
	public required string Image { get; set; }
}