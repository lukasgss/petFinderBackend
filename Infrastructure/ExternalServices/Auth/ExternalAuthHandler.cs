using System.Net.Http.Json;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Authorization.Facebook;
using Application.Common.Interfaces.Authorization.Google;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.Auth;

public class ExternalAuthHandler : IExternalAuthHandler
{
	private readonly GoogleAuthConfig _googleAuthConfig;
	private readonly FacebookAuthConfig _facebookAuthConfig;
	private readonly IHttpClientFactory _httpClientFactory;

	public ExternalAuthHandler(IOptions<GoogleAuthConfig> googleAuthConfig,
		IOptions<FacebookAuthConfig> facebookAuthConfig,
		IHttpClientFactory httpClientFactory)
	{
		_googleAuthConfig = googleAuthConfig.Value ?? throw new ArgumentNullException(nameof(googleAuthConfig));
		_facebookAuthConfig = facebookAuthConfig.Value ?? throw new ArgumentNullException(nameof(facebookAuthConfig));
		_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
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

	public async Task<FacebookPayload?> ValidateFacebookToken(ExternalAuthRequest externalAuth)
	{
		try
		{
			HttpClient httpClient = _httpClientFactory.CreateClient(FacebookGraphApiConfig.ClientKey);
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {externalAuth.IdToken}");

			var userDataResponse =
				await httpClient.GetFromJsonAsync<FacebookUserDataResponse>("/me?fields=id,name,email,picture");
			if (userDataResponse is null)
			{
				return null;
			}

			httpClient.DefaultRequestHeaders.Clear();
			return new FacebookPayload()
			{
				UserId = userDataResponse.Id,
				FullName = userDataResponse.Name,
				Email = userDataResponse.Email,
				Image = userDataResponse.Picture.Data.Url,
			};
		}
		catch (Exception ex)
		{
			ClearHttpClientHeaders();
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	private void ClearHttpClientHeaders()
	{
		HttpClient httpClient = _httpClientFactory.CreateClient(FacebookGraphApiConfig.ClientKey);
		httpClient.DefaultRequestHeaders.Clear();
	}
}