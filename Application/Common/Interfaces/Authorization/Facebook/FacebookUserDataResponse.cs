using System.Text.Json.Serialization;

namespace Application.Common.Interfaces.Authorization.Facebook;

public class FacebookUserDataResponse
{
	[JsonPropertyName("id")]
	public required string Id { get; init; }

	[JsonPropertyName("name")]
	public required string Name { get; init; }

	[JsonPropertyName("email")]
	public required string Email { get; init; }

	[JsonPropertyName("picture")]
	public required PictureData Picture { get; init; }
}

public class PictureData
{
	[JsonPropertyName("data")]
	public required Picture Data { get; init; }
}

public class Picture
{
	[JsonPropertyName("height")]
	public required int Height { get; init; }

	[JsonPropertyName("width")]
	public required int Width { get; init; }

	[JsonPropertyName("is_silhouette")]
	public required bool IsSilhouette { get; init; }

	[JsonPropertyName("url")]
	public required string Url { get; init; }
}