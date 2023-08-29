using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly string _audience;
    private readonly string _issuer;
    private readonly int _expiryInMinutes;

    public JwtTokenGenerator(string secretKey, string audience, string issuer, int expiryInMinutes)
    {
        _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
        _audience = audience ?? throw new ArgumentNullException(nameof(audience));
        _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        _expiryInMinutes = expiryInMinutes;
    }

    public string GenerateToken(Guid userId, string fullName)
    {
        SigningCredentials signingCredentials = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, fullName)
        };

        JwtSecurityToken jwtSecurityToken = new(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_expiryInMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}