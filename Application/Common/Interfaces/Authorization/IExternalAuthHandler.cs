using Application.Common.Interfaces.Authorization.Facebook;
using Application.Common.Interfaces.Authorization.Google;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Authorization;

public interface IExternalAuthHandler
{
	Task<GooglePayload?> ValidateGoogleToken(ExternalAuthRequest externalAuth);
	Task<FacebookPayload?> ValidateFacebookToken(ExternalAuthRequest externalAuth);
}