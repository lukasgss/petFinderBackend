namespace Application.Common.Interfaces.ExternalServices;

public class AwsS3ImageResponse
{
    public bool Success { get; init; }
    public string? PublicUrl { get; init; }
}