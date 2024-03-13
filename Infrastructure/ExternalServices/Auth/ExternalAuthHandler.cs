using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.Auth;

public class ExternalAuthHandler : IExternalAuthHandler
{
	private readonly GoogleAuthConfig _googleAuthConfig;

	public ExternalAuthHandler(IOptions<GoogleAuthConfig> googleAuthConfig)
	{
		_googleAuthConfig = googleAuthConfig.Value;
	}

	public async Task<GooglePayload?> ValidateGoogleToken(ExternalAuthRequest externalAuth)
	{
		try
		{
			GoogleJsonWebSignature.ValidationSettings settings = new()
			{
				Audience = new List<string>() { _googleAuthConfig.ClientId }
			};
			GoogleJsonWebSignature.Payload payload =
				await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);

			return new GooglePayload()
			{
				Email = payload.Email,
				Subject = payload.Subject,
				FullName = $"{payload.Name}{payload.FamilyName}",
				Image = payload.Picture
			};
		}
		catch
		{
			return null;
		}
	}
}