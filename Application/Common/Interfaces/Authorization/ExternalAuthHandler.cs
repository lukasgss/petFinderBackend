using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Authorization;

public interface IExternalAuthHandler
{
	Task<GooglePayload?> ValidateGoogleToken(ExternalAuthRequest externalAuth);
}