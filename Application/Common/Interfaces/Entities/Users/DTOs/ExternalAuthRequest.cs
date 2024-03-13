namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class ExternalAuthRequest
{
	public required string Provider { get; set; }
	public required string IdToken { get; set; }
}