using System.Net.Http.Json;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Authorization.Facebook;
using Application.Common.Interfaces.Authorization.Google;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.Auth;

public class ExternalAuthHandler : IExternalAuthHandler
{
	private readonly GoogleAuthConfig _googleAuthConfig;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger<ExternalAuthHandler> _logger;
	private static readonly string[] Scopes = { "https://www.googleapis.com/auth/userinfo.email" };
	private readonly GoogleAuthorizationCodeFlow _flow;

	public ExternalAuthHandler(IOptions<GoogleAuthConfig> googleAuthConfig,
		IHttpClientFactory httpClientFactory,
		ILogger<ExternalAuthHandler> logger)
	{
		_googleAuthConfig = googleAuthConfig.Value ?? throw new ArgumentNullException(nameof(googleAuthConfig));
		_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));

		_flow = new GoogleAuthorizationCodeFlow(
			new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets
				{
					ClientId = _googleAuthConfig.ClientId,
					ClientSecret = _googleAuthConfig.ClientSecret
				},
				Scopes = Scopes,
				DataStore = null
			});
	}

	public async Task<ExternalAuthPayload?> ValidateGoogleToken(ExternalAuthRequest externalAuth)
	{
		try
		{
			TokenResponse? token = await _flow.ExchangeCodeForTokenAsync(
				string.Empty,
				externalAuth.IdToken,
				"https://localhost:5173",
				CancellationToken.None);

			bool isValidIdToken = await IdTokenIsValid(token.IdToken);
			if (!isValidIdToken)
			{
				return null;
			}

			UserCredential credential = new(_flow, externalAuth.IdToken, token);
			Oauth2Service userInfoService = new(new BaseClientService.Initializer
				{ HttpClientInitializer = credential });
			Userinfo? userInfo = await userInfoService.Userinfo.Get().ExecuteAsync();

			return new ExternalAuthPayload()
			{
				Email = userInfo.Email,
				UserId = userInfo.Id,
				Image = userInfo.Picture,
				FullName = $"{userInfo.Name} {userInfo.FamilyName}"
			};
		}
		catch (TokenResponseException ex)
		{
			_logger.LogError("Failed to exchange code for token: {Exception}", ex);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError("Failed to process OAuth: {Exception}", ex);
			return null;
		}
	}

	public async Task<ExternalAuthPayload?> ValidateFacebookToken(ExternalAuthRequest externalAuth)
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
			return new ExternalAuthPayload()
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

	private async Task<bool> IdTokenIsValid(string idToken)
	{
		try
		{
			GoogleJsonWebSignature.ValidationSettings settings = new()
			{
				Audience = new List<string>() { _googleAuthConfig.ClientId }
			};
			await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError("Error validating id token: {Exception}", ex);
			return false;
		}
	}

	private void ClearHttpClientHeaders()
	{
		HttpClient httpClient = _httpClientFactory.CreateClient(FacebookGraphApiConfig.ClientKey);
		httpClient.DefaultRequestHeaders.Clear();
	}
}