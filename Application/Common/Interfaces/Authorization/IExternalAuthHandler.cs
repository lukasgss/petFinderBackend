using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Authorization;

public interface IExternalAuthHandler
{
	Task<ExternalAuthPayload?> ValidateGoogleToken(ExternalAuthRequest externalAuth);
	Task<ExternalAuthPayload?> ValidateFacebookToken(ExternalAuthRequest externalAuth);
}