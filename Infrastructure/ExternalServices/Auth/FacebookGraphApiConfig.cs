namespace Infrastructure.ExternalServices.Auth;

public static class FacebookGraphApiConfig
{
	public const string ClientKey = "FacebookGraph";
	public static readonly Uri BaseUrl = new("https://graph.facebook.com");
}