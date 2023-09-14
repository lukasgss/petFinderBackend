namespace Application.Common.Interfaces.Authentication;

public class JwtConfig
{
    public const string SectionName = "JwtSettings";
    
    public string SecretKey { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public int ExpiryTimeInMin { get; init; }
}