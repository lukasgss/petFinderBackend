using System.Net.Http.Json;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Authorization.Facebook;
using Application.Common.Interfaces.Authorization.Google;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.Auth;

public class ExternalAuthHandler : IExternalAuthHandler
{
	private readonly GoogleAuthConfig _googleAuthConfig;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger<ExternalAuthHandler> _logger;

	public ExternalAuthHandler(IOptions<GoogleAuthConfig> googleAuthConfig,
		IHttpClientFactory httpClientFactory,
		ILogger<ExternalAuthHandler> logger)
	{
		_googleAuthConfig = googleAuthConfig.Value ?? throw new ArgumentNullException(nameof(googleAuthConfig));
		_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
			_logger.LogError("{Exception}", ex);
			return null;
		}
	}

	private void ClearHttpClientHeaders()
	{
		HttpClient httpClient = _httpClientFactory.CreateClient(FacebookGraphApiConfig.ClientKey);
		httpClient.DefaultRequestHeaders.Clear();
	}
}