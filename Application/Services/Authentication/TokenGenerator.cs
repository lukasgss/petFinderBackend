using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Interfaces.Providers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Authentication;

public class TokenGenerator : ITokenGenerator
{
	private readonly IValueProvider _valueProvider;
	private readonly JwtConfig _jwtConfig;

	public TokenGenerator(IValueProvider valueProvider, IOptions<JwtConfig> jwtConfig)
	{
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
	}

	public TokensResponse GenerateTokens(Guid userId, string fullName)
	{
		Claim[] accessTokenClaims = GenerateTokenClaims(userId, fullName, TokenTypeNames.AccessToken);
		DateTime accessTokenExpiration = _valueProvider.Now()
			.AddMinutes(_jwtConfig.AccessTokenExpiryTimeInMin);
		string accessToken = GenerateJwtToken(accessTokenClaims, accessTokenExpiration);

		Claim[] refreshTokenClaims = GenerateTokenClaims(userId, fullName, TokenTypeNames.RefreshToken);
		DateTime refreshTokenExpiration = _valueProvider.Now()
			.AddMinutes(_jwtConfig.RefreshTokenExpiryTimeInMin);

		string refreshToken = GenerateJwtToken(refreshTokenClaims, refreshTokenExpiration);

		return new TokensResponse()
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}

	private static Claim[] GenerateTokenClaims(Guid userId, string fullName, string tokenType)
	{
		return new[]
		{
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Name, fullName),
			new Claim("token_type", tokenType)
		};
	}

	private string GenerateJwtToken(IEnumerable<Claim> claims, DateTime expirationDate)
	{
		SigningCredentials signingCredentials = new(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey)),
			SecurityAlgorithms.HmacSha256);

		JwtSecurityToken jwtSecurityToken = new(
			issuer: _jwtConfig.Issuer,
			audience: _jwtConfig.Audience,
			claims: claims,
			expires: expirationDate,
			signingCredentials: signingCredentials);

		return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
	}
}